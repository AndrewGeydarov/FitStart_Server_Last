namespace FitStart_Server.Requests
{
    public class PersonalTrainingModel
    {
        public int UserID { get; set; }
        public int TrainerID { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int DurationMinutes { get; set; }
    }
}
