namespace FitStart_Server.Requests
{
    public class PurchaseSubscriptionModel
    {
        public int UserID { get; set; }
        public int SubscriptionID { get; set; }
        public int ClubID { get; set; }
        public string PaymentMethod { get; set; }
        public string PromoAction { get; set; }
    }
}
