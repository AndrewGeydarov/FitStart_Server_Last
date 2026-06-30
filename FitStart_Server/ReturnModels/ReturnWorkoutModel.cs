using FitStart_Server.Models;

namespace FitStart_Server.ReturnModels
{
    public class ReturnWorkoutModel
    {
        public int WorkoutID { get; set; }
        public string WorkoutName { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int Intensity { get; set; }
        public string PreviewPath { get; set; }
        public string VideoPath { get; set; }
        public string TrainerName { get; set; }
        public bool isTrial { get; set; }
        public Workout_Type WorkoutType { get; set; }
        public bool isFavorite { get; set; }
    }
}
