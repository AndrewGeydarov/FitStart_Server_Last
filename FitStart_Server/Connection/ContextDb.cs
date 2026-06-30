using FitStart_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Connection
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions options) : base(options) { }

        public DbSet<Banner> Banners { get; set; }
        public DbSet<Body_Composition> BodyCompositions { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Club_Load> ClubLoads { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Exercise_Description> ExerciseDescriptions { get; set; }
        public DbSet<Exercise_Type> ExerciseTypes { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Muscle_Group> MuscleGroups { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Pass> Passes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Personal_Training> PersonalTrainings { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Subscription_Freeze> SubscriptionFreezes { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerCategory> TrainerCategories { get; set; }
        public DbSet<Training_Diary> TrainingDiaries { get; set; }
        public DbSet<Training_Equipment> TrainingEquipments { get; set; }
        public DbSet<TrainingEquip_Type> TrainingEquipTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<User_Favorite_Workout> UserFavoriteWorkouts { get; set; }
        public DbSet<User_Subscription> UserSubscriptions { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Workout_Type> WorkoutTypes { get; set; }
    }
}
