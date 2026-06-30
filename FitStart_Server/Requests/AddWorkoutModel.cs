namespace FitStart_Server.Requests
{
    public class AddWorkoutModel
    {
        public string WorkoutName { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int Intensity { get; set; }
        public string TrainerName { get; set; }
        public bool isTrial { get; set; }
        public int WT_ID { get; set; }
    }
}
