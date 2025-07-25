using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;

namespace All4GYM.Services;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name));

        CreateMap<TrainingProgram, TrainingProgramDto>();
        CreateMap<Workout, WorkoutDto>();
        CreateMap<Exercise, ExerciseDto>();
        CreateMap<CreateExerciseDto, Exercise>();

        CreateMap<MealLog, MealLogDto>()
            .ForMember(dest => dest.FoodItemName, opt => opt.MapFrom(src => src.FoodItem.Name));
        CreateMap<CreateMealLogDto, MealLog>();

        CreateMap<Subscription, SubscriptionDto>();
        CreateMap<CreateSubscriptionDto, Subscription>();

        CreateMap<ProgressLog, ProgressLogDto>();
        CreateMap<CreateProgressLogDto, ProgressLog>();

        CreateMap<VideoContent, VideoContentDto>();
        CreateMap<CreateVideoContentDto, VideoContent>();

        CreateMap<ShopProduct, ShopProductDto>();
        CreateMap<CreateShopProductDto, ShopProduct>();
        
        CreateMap<CreateFoodItemDto, FoodItem>();
        CreateMap<FoodItem, FoodItemDto>();

        CreateMap<Booking, BookingDto>();
        CreateMap<CreateBookingDto, Booking>();

        CreateMap<GroupSession, GroupSessionDto>();

        //CreateMap<Order, OrderDto>();
        //CreateMap<OrderProduct, OrderProductDto>();
        
        CreateMap<Recipe, RecipeDto>();
        CreateMap<CreateRecipeDto, Recipe>();
        
        CreateMap<UpdateUserProfileDto, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}