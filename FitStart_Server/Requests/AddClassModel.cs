namespace FitStart_Server.Requests
{
    public class AddClassModel
    {
        public string ClassName { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int Intensity { get; set; }
    }
}
