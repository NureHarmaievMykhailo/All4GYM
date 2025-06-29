using Microsoft.EntityFrameworkCore;
using All4GYM.Models;

namespace All4GYM.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<TrainingProgram> TrainingPrograms { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<MealLog> MealLogs { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Gym> Gyms { get; set; }
    public DbSet<GroupSession> GroupSessions { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<ProgressLog> ProgressLogs { get; set; }
    public DbSet<VideoContent> VideoContents { get; set; }
    public DbSet<ShopProduct> ShopProducts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite keys
        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => new { op.OrderId, op.ProductId });

        modelBuilder.Entity<WorkoutExercise>()
            .HasKey(we => new { we.WorkoutId, we.ExerciseId });

        // Вимикаємо каскадне видалення для користувачів у сутностях, що викликають конфлікт
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GroupSession>()
            .HasOne(gs => gs.Trainer)
            .WithMany()
            .HasForeignKey(gs => gs.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MealLog>()
            .HasOne(m => m.User)
            .WithMany(u => u.MealLogs)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProgressLog>()
            .HasOne(p => p.User)
            .WithMany(u => u.ProgressLogs)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TrainingProgram>()
            .HasOne(tp => tp.User)
            .WithMany(u => u.TrainingPrograms)
            .HasForeignKey(tp => tp.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
