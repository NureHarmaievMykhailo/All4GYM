using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using All4GYM.Data;
using All4GYM.Services;
using All4GYM.Services.Stripe;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Stripe ключ
Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Swagger + JWT підтримка
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "All4GYM API",
        Version = "1.0",
        Description = "Backend API для All4GYM",
        Contact = new OpenApiContact
        {
            Name = "All4GYM Team",
            Email = "support@all4gym.com"
        }
    });

    c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Введіть ваш JWT токен у форматі: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// База даних
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Сервіси
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITrainingProgramService, TrainingProgramService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IWorkoutExerciseService, WorkoutExerciseService>();
builder.Services.AddScoped<IMealLogService, MealLogService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IProgressLogService, ProgressLogService>();
builder.Services.AddScoped<IVideoContentService, VideoContentService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSingleton<StripePaymentIntentService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<IGroupSessionService, GroupSessionService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<IFoodItemService, FoodItemService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Контролери
builder.Services.AddControllers();

// Аутентифікація JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        options.MapInboundClaims = false;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("jwt", out var jwt))
                {
                    context.Token = jwt;
                }
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is ClaimsIdentity identity)
                {
                    var jwtToken = context.SecurityToken as JwtSecurityToken;
                    if (jwtToken != null)
                    {
                        // Витягуємо claims напряму з JWT
                        var hasActive = jwtToken.Claims.FirstOrDefault(c => c.Type == "HasActiveSubscription")?.Value;
                        var tier = jwtToken.Claims.FirstOrDefault(c => c.Type == "SubscriptionTier")?.Value;

                        if (!string.IsNullOrEmpty(hasActive))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.UserData, hasActive));
                        }

                        if (!string.IsNullOrEmpty(tier))
                        {
                            identity.AddClaim(new Claim("CustomTier", tier));
                        }
                    }
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// Статичні файли
app.UseStaticFiles();

// Додаємо cookie → JWT токен у заголовок
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["jwt"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers["Authorization"] = $"Bearer {token}";
    }

    await next();
});

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "All4GYM API v1");
    c.RoutePrefix = "swagger";
});

// Middleware
// app.UseHttpsRedirection(); // вимкнено для локального HTTP
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
