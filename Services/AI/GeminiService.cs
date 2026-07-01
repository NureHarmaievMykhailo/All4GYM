using All4GYM.Data;
using All4GYM.Dtos.AI;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace All4GYM.Services.AI;

public class GeminiService : IAIService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiService(AppDbContext context, HttpClient httpClient, IMapper mapper, IConfiguration config)
    {
        _context = context;
        _httpClient = httpClient;
        _mapper = mapper;
        _apiKey = config["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini API Key missing");
        _model = config["Gemini:Model"] ?? "gemini-2.5-flash";
    }

    public async Task<AIAnalysisResultDto> GenerateReviewAsync(int userId, AIAnalysisRequestDto dto)
    {
        var user = await _context.Users.FindAsync(userId) 
            ?? throw new KeyNotFoundException("User not found");

        var startDate = DateTime.UtcNow.AddDays(-dto.PeriodDays);
        string prompt = "";
        
        if (dto.VectorType.Equals("Nutrition", StringComparison.OrdinalIgnoreCase))
        {
            var meals = await _context.MealLogs
                .Where(m => m.UserId == userId && m.Date >= startDate)
                .ToListAsync();

            var weights = await _context.ProgressLogs
                .Where(p => p.UserId == userId && p.Date >= startDate)
                .OrderBy(p => p.Date)
                .ToListAsync();

            prompt = FormulateNutritionPrompt(user, meals, weights, dto.PeriodDays);
        }
        else
        {
            var workouts = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .Where(w => w.UserId == userId && w.Date >= startDate)
                .ToListAsync();

            prompt = FormulateWorkoutPrompt(user, workouts, dto.PeriodDays);
        }
        
        var aiRawJson = await FetchGeminiResponseAsync(prompt);
        
        using var doc = JsonDocument.Parse(aiRawJson);
        var root = doc.RootElement;

        var review = new AIReview
        {
            UserId = userId,
            VectorType = dto.VectorType,
            GeneratedAt = DateTime.UtcNow,
            PeriodDays = dto.PeriodDays,
            Overview = root.GetProperty("Overview").GetString() ?? "",
            RecommendationsJson = root.GetProperty("Recommendations").GetRawText(),
            TrendPrediction = root.GetProperty("TrendPrediction").GetString() ?? ""
        };

        _context.AIReviews.Add(review);
        await _context.SaveChangesAsync();

        return _mapper.Map<AIAnalysisResultDto>(review);
    }

    public async Task<bool> SubmitFeedbackAsync(int userId, SubmitFeedbackDto dto)
    {
        var review = await _context.AIReviews.FindAsync(dto.AIReviewId);
        if (review == null || review.UserId != userId) return false;

        var feedback = new UserFeedback
        {
            AIReviewId = dto.AIReviewId,
            UserId = userId,
            IsHelpful = dto.IsHelpful,
            UserComments = dto.UserComments,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserFeedbacks.Add(feedback);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<AIAnalysisResultDto>> GetUserReviewHistoryAsync(int userId, string vectorType)
    {
        var history = await _context.AIReviews
            .Where(r => r.UserId == userId && r.VectorType.ToLower() == vectorType.ToLower())
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();

        return _mapper.Map<List<AIAnalysisResultDto>>(history);
    }
    
    public async Task<WorkoutOptimizationResultDto> OptimizeWorkoutAsync(int userId, int currentWorkoutId)
    {
        var currentWorkout = await _context.Workouts
            .Include(w => w.WorkoutExercises).ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(w => w.Id == currentWorkoutId && w.UserId == userId);

        if (currentWorkout == null) throw new KeyNotFoundException("Тренування не знайдено");

        var previousWorkouts = await _context.Workouts
            .Include(w => w.WorkoutExercises).ThenInclude(we => we.Exercise)
            .Where(w => w.UserId == userId && w.TrainingProgramId == currentWorkout.TrainingProgramId && w.Date < currentWorkout.Date)
            .OrderByDescending(w => w.Date)
            .Take(2)
            .ToListAsync();
        
        string prompt = FormulateOptimizationPrompt(currentWorkout, previousWorkouts);
        var resultDto = await FetchOptimizationJsonAsync(prompt);

        return resultDto;
    }
    
    public async Task<DailyNutritionAnalysisDto> AnalyzeDailyNutritionAsync(int userId, DateTime targetDate)
    {
        var mealLogs = await _context.MealLogs
            .Include(m => m.FoodItem)
            .Where(m => m.UserId == userId && m.Date.Date == targetDate.Date)
            .ToListAsync();
        var user = await _context.Users
                       .FirstOrDefaultAsync(u => u.Id == userId) 
                   ?? throw new Exception("Користувача не знайдено");
        string prompt = FormulateDailyNutritionPrompt(targetDate, mealLogs, user);
        return await FetchDailyNutritionJsonAsync(prompt);
    }

    #region Промпти та HTTP Клієнт

    private string FormulateNutritionPrompt(User user, List<MealLog> meals, List<ProgressLog> weights, int days)
    {
        var mealsSummary = meals.GroupBy(m => m.Date.Date)
            .Select(g => $"Дата: {g.Key:yyyy-MM-dd}, Калории: {g.Sum(m => m.Calories)}, Б: {g.Sum(m => m.Proteins)}г, Ж: {g.Sum(m => m.Fats)}г, У: {g.Sum(m => m.Carbs)}г");

        var weightSummary = weights.Select(p => $"Дата: {p.Date:yyyy-MM-dd}, Вес: {p.Weight}кг, Заметки: {p.Notes}");

        return $@"Ты — ИИ-тренер и диетолог в системе All4GYM. Проанализируй питание за {days} дней.
        Профиль: Пол: {user.Gender}, Возраст: {user.Age}, Рост: {user.HeightCm}см, Текущий вес: {user.WeightKg}кг.
        Логи еды:\n{string.Join("\n", mealsSummary)}
        История взвешиваний:\n{string.Join("\n", weightSummary)}
        Выдай глубокую аналитику метаболического равновесия и плотности макронутриентов. Найди скрытые тенденции (связь между калориями, заметками и колебаниями веса). Дай 3 точных совета.";
    }

    private string FormulateWorkoutPrompt(User user, List<Workout> workouts, int days)
    {
        var workoutSummary = workouts.Select(w => 
            $"Дата: {w.Date:yyyy-MM-dd}, Упражнения: " + string.Join(", ", w.WorkoutExercises.Select(we => $"{we.Exercise.Name} ({we.Weight}кг x {we.Reps})")));

        return $@"Ти — ИИ-эксперт по биомеханике в All4GYM. Проанализируй тренировки за {days} дней.
        Профиль: Рост: {user.HeightCm}см, Вес: {user.WeightKg}кг.
        Логи тренировок:\n{string.Join("\n", workoutSummary)}
        Проанализируй прогрессию нагрузок (Progressive Overload) и объемный тренинг. Оцени, эффективен ли шаг весов. Дай 3 тактических совета.";
    }

    private async Task<string> FetchGeminiResponseAsync(string prompt)
    {
        var relativeUrl = $"models/{_model}:generateContent?key={_apiKey}";

        var schemaJson = @"{
        ""type"": ""object"",
        ""properties"": {
            ""Overview"":        { ""type"": ""string"" },
            ""Recommendations"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } },
            ""TrendPrediction"": { ""type"": ""string"" }
        },
        ""required"": [""Overview"", ""Recommendations"", ""TrendPrediction""]
    }";
        
        var requestJson = $@"{{
        ""contents"": [{{
            ""parts"": [{{ ""text"": {JsonSerializer.Serialize(prompt)} }}]
        }}],
        ""generationConfig"": {{
            ""responseMimeType"": ""application/json"",
            ""responseSchema"": {schemaJson}
        }}
    }}";

        var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(relativeUrl, httpContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Gemini API Error ({(int)response.StatusCode}): {errorContent}");
        }

        var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>();

        var rawTextResponse = jsonResult
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return rawTextResponse ?? throw new Exception("Gemini returned empty response");
    }
    
    private string FormulateOptimizationPrompt(Workout current, List<Workout> previous) 
{ 
    var sb = new StringBuilder(); 
    
    sb.AppendLine("Роль: Ти — AI-експерт з біомеханіки, спортивної медицини та побудови тренувань в All4GYM.");
    sb.AppendLine("Завдання: Проаналізувати поточне тренування, порівняти з минулими, знайти плато/помилки та дати інструкцію.");
    sb.AppendLine("Правило відповіді: Без «води», без привітань, строго по факту. Тільки конкретні зауваження та цифри.");
    sb.AppendLine("---");
    
    sb.AppendLine($"ПОТОЧНЕ ТРЕНУВАННЯ ({current.Date:yyyy-MM-dd}):"); 
    foreach (var we in current.WorkoutExercises) 
    {
        sb.AppendLine($" - {we.Exercise.Name} [{we.Exercise.MuscleGroup}]: {we.Sets} підх. х {we.Reps} повт. х {we.Weight}кг");
    } 
    
    if (previous != null && previous.Any()) 
    { 
        sb.AppendLine("\nІСТОРІЯ МИНУЛИХ ТРЕНУВАНЬ (для аналізу прогресії та плато):"); 
        foreach (var w in previous.OrderByDescending(x => x.Date)) 
        { 
            sb.AppendLine($" [{w.Date:yyyy-MM-dd}]:"); 
            foreach (var we in w.WorkoutExercises) 
            { 
                sb.AppendLine($"   • {we.Exercise.Name} [{we.Exercise.MuscleGroup}]: {we.Sets}х{we.Reps}х{we.Weight}кг"); 
            } 
        } 
    } 
    else 
    { 
        sb.AppendLine("\nІсторія минулих тренувань відсутня. Проаналізуй тільки поточну структуру."); 
    } 
    
    sb.AppendLine("\n---");
    sb.AppendLine("КРИТЕРІЇ АНАЛІЗУ:");
    sb.AppendLine("1. Порядок вправ та чергування: Чи йдуть важкі (базові) вправи першими? Чи немає перевантаження однієї групи м'язів підряд (наприклад, жим штанги одразу після жиму гантелей)?");
    sb.AppendLine("2. Аналіз прогресу/плато: Порівняй робочі веса та повторення в однакових вправах. Якщо показники не ростуть або падають 2+ тренування поспіль — зафіксуй плато.");
    sb.AppendLine("3. Чіткий план дій: Що конкретно змінити (вага, повтори, порядок) на наступному тренуванні.");
    
    sb.AppendLine("\nФОРМАТ ВІДПОВІДІ (Дотримуйся його суворо):");
    sb.AppendLine("⚠️ Помилки/Чергування: [Коротко, що не так з порядком або групами м'язів, або 'Порядок оптимізовано']");
    sb.AppendLine("📈 Статус прогресу: [Виявлені плато або фіксація прогресу у конкретних вправах]");
    sb.AppendLine("🎯 План на наступний раз: [Точні рекомендації: змінити вагу на Х кг, поміняти місцями вправу А і Б]");

    return sb.ToString(); 
}

    private async Task<WorkoutOptimizationResultDto> FetchOptimizationJsonAsync(string prompt)
    {
        var relativeUrl = $"models/{_model}:generateContent?key={_apiKey}";
        var schemaJson = @"{
            ""type"": ""object"",
            ""properties"": {
                ""OrderOptimization"": { ""type"": ""string"" },
                ""PlateauDetection"":  { ""type"": ""string"" },
                ""FuturePlan"":          { ""type"": ""string"" }
            },
            ""required"": [""OrderOptimization"", ""PlateauDetection"", ""FuturePlan""]
        }";

        var requestJson = $@"{{
            ""contents"": [{{
                ""parts"": [{{ ""text"": {JsonSerializer.Serialize(prompt)} }}]
            }}],
            ""generationConfig"": {{
                ""responseMimeType"": ""application/json"",
                ""responseSchema"": {schemaJson}
            }}
        }}";

        var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(relativeUrl, httpContent);

        if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Gemini Error: {await response.Content.ReadAsStringAsync()}");

        var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>();

        var rawTextResponse = jsonResult
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "{}";
        
        return JsonSerializer.Deserialize<WorkoutOptimizationResultDto>(rawTextResponse) 
               ?? new WorkoutOptimizationResultDto();
    }

    private string FormulateDailyNutritionPrompt(DateTime targetDate, List<MealLog> meals, User user)
{
    var sb = new StringBuilder();
    sb.AppendLine($"Дата аналізу: {targetDate:yyyy-MM-dd}");
    
    // Вытягиваем таргеты (убедись, что названия свойств совпадают с твоей моделью User)
    int targetCal = user.TargetCalories;
    float targetP = user.TargetProteins;
    float targetF = user.TargetFats;
    float targetC = user.TargetCarbs;

    sb.AppendLine("\n🎯 ЦІЛЬОВІ НОРМИ КОРИСТУВАЧА НА ДЕНЬ:");
    sb.AppendLine($"- Калорії: {targetCal} ккал | Білки: {targetP}г | Жири: {targetF}г | Вуглеводи: {targetC}г\n");

    // Считаем фактическое потребление через LINQ
    float totalCal = meals.Sum(m => m.Calories);
    float totalP = meals.Sum(m => m.Proteins);
    float totalF = meals.Sum(m => m.Fats);
    float totalC = meals.Sum(m => m.Carbs);

    if (!meals.Any())
    {
        sb.AppendLine("З'ЇДЕНО: Користувач ще не додав жодного прийому їжі за цей день.");
    }
    else
    {
        sb.AppendLine("З'ЇДЕНО ПРОДУКТИ:");
        foreach (var m in meals)
        {
            sb.AppendLine($"- [{m.MealType}] {m.FoodItem?.Name ?? "Продукт"}: {m.Grams}г ({m.Calories} ккал, Б: {m.Proteins}г, Ж: {m.Fats}г, В: {m.Carbs}г)");
        }
    }

    // Считаем точную математическую разницу
    float diffCal = targetCal - totalCal;
    float diffP = targetP - totalP;
    float diffF = targetF - totalF;
    float diffC = targetC - totalC;

    sb.AppendLine("\n📊 ПОТОЧНИЙ БАЛАНС:");
    sb.AppendLine($"- Калорії: {totalCal} з {targetCal} ккал (Залишок: {diffCal} ккал)");
    sb.AppendLine($"- Білки: {totalP} з {targetP}г (Залишок: {diffP}г)");
    sb.AppendLine($"- Жири: {totalF} з {targetF}г (Залишок: {diffF}г)");
    sb.AppendLine($"- Вуглеводи: {totalC} з {targetC}г (Залишок: {diffC}г)");

    sb.AppendLine(@"
Ти — професійний AI-нутриціолог платформи All4GYM. Твоя задача - проаналізувати поточний раціон користувача, порівнюючи його з ЦІЛЬОВИМИ НОРМАМИ.

КРИТИЧНІ ПРАВИЛА ДЛЯ РЕКОМЕНДАЦІЙ (RecommendationForDinner):
1. ПРІОРИТЕТ КАЛОРІЙ: Якщо у користувача ВЖЕ є надлишок калорій (Залишок калорій < 0) або до ліміту залишилося менше 50 ккал, ти НАЙПОТУЖНІШЕ ЗАБОРОНЯЄШ будь-яку їжу, навіть якщо не дообрано білок! 
   - У цьому випадку твоя рекомендація має звучати так: 'Сьогодні ліміт калорій уже вичерпано (перебір на Х ккал). Рекомендується утриматися від додаткових прийомів їжі. Щоб закрити потребу в білку без зайвих калорій на майбутнє, перенесіть високобілкові продукти на першу половину дня, а вечір залиште для відпочинку/води.'
2. Якщо калорії ще дозволяють (є запас), але є дефіцит білка - пропонуй легкі білкові продукти (сир 0%, філе, ізолят), які МАТЕМАТИЧНО вписуються у ЗАЛИШОК калорій. Продукт не повинен викликати перебір по калоріях!
3. Якщо всі норми (і калорії, і макроси) плюс-мінус виконано - похвали користувача та дай пораду щодо підтримання такого балансу (наприклад, додати більше зелені або стежити за гідратацією).
4. Якщо э критичний недостаток по калорія - порекомендуй користувачу терміново поїсти збалансовані один або декілька (в халежності від поточної ситуації по калорія) прийомів їжі
Вимоги до полів у JSON:
1. BalanceVerdict: Оцінка балансу КБЖУ, спираючись на точні цифри залишку або перевищення норми. Якщо є перебір калорій - чітко вкажи на це.
2. MicronutrientsNotes: Аналіз якості обраних продуктів (чи немає надлишку швидких цукрів, шкідливих жирів, фастфуду).
3. RecommendationForDinner: Коротка, математично обґрунтована порада на вечір/наступний прийом їжі строго з урахуванням ПРІОРИТЕТУ КАЛОРІЙ (див. правила вище).

Відповідь має бути короткою, експертною та без води.");

    return sb.ToString();
}

    private async Task<DailyNutritionAnalysisDto> FetchDailyNutritionJsonAsync(string prompt)
    {
        var relativeUrl = $"models/{_model}:generateContent?key={_apiKey}";
        
        var schemaJson = @"{
            ""type"": ""object"",
            ""properties"": {
                ""balanceVerdict"":          { ""type"": ""string"" },
                ""micronutrientsNotes"":     { ""type"": ""string"" },
                ""recommendationForDinner"": { ""type"": ""string"" }
            },
            ""required"": [""balanceVerdict"", ""micronutrientsNotes"", ""recommendationForDinner""]
        }";

        var requestJson = $@"{{
            ""contents"": [{{
                ""parts"": [{{ ""text"": {JsonSerializer.Serialize(prompt)} }}]
            }}],
            ""generationConfig"": {{
                ""responseMimeType"": ""application/json"",
                ""responseSchema"": {schemaJson}
            }}
        }}";

        var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(relativeUrl, httpContent);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Gemini Error]: {error}");
            throw new HttpRequestException("Помилка генерації AI-аналізу");
        }

        var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        var rawTextResponse = jsonResult
            .GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "{}";

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<DailyNutritionAnalysisDto>(rawTextResponse, options) 
               ?? new DailyNutritionAnalysisDto();
    }
    
    #endregion
}