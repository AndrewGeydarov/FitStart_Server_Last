namespace FitStart_Server.Requests
{
    public class AddSubscriptionModel
    {
        public string SubscriptionName { get; set; }
        public double Cost { get; set; }
        public int DurationDays { get; set; }
        public string Description { get; set; }
        public double EntryFee { get; set; }
    }
}
