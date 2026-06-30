namespace FitStart_Server.Requests
{
    public class ScheduleFilterModel
    {
        public DateOnly? Date { get; set; }
        public int? ClubID { get; set; }
        public TimeOnly? TimeFrom { get; set; }
        public TimeOnly? TimeTo { get; set; }
        public int? Intensity { get; set; }
    }
}
