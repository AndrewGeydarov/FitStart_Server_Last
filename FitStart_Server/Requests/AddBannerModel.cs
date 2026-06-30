namespace FitStart_Server.Requests
{
    public class AddBannerModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ActionLink { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
