using FitStart_Server.Models;

namespace FitStart_Server.ReturnModels
{
    public class ReturnScheduleItemModel
    {
        public int ScheduleID { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public int Intensity { get; set; }
        public Trainer Trainer { get; set; }
        public Club Club { get; set; }
        public int MaxSlots { get; set; }
        public int FreeSlots { get; set; }
        public bool isUserSignedUp { get; set; }
    }
}
