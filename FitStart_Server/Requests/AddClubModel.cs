namespace FitStart_Server.Requests
{
    public class AddClubModel
    {
        public string ClubName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
    }
}
