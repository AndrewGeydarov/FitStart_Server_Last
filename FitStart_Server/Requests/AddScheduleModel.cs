namespace FitStart_Server.Requests
{
    public class AddScheduleModel
    {
        public int TrainerID { get; set; }
        public int ClassID { get; set; }
        public int ClubID { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int MaxSlots { get; set; }
    }
}
