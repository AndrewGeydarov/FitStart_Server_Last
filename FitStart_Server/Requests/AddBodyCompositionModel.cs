namespace FitStart_Server.Requests
{
    public class AddBodyCompositionModel
    {
        public int UserID { get; set; }
        public DateOnly MeasureDate { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BodyFatPercent { get; set; }
        public double MuscleMass { get; set; }
        public double WaterPercent { get; set; }
    }
}
