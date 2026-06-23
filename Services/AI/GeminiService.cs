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
                .Where(w => w.TrainingProgram.UserId == userId && w.Date >= startDate)
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

    #region Промпты и HTTP Клиент

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

        return $@"Ты — ИИ-эксперт по биомеханике в All4GYM. Проанализируй тренировки за {days} дней.
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

    #endregion
}