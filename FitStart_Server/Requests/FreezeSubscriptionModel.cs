namespace FitStart_Server.Requests
{
    public class FreezeSubscriptionModel
    {
        public int US_ID { get; set; }
        public DateOnly FreezeStart { get; set; }
        public int FreezeDays { get; set; }
    }
}
