using FitStart_Server.Models;

namespace FitStart_Server.ReturnModels
{
    public class ReturnSubscriptionInfoModel
    {
        public int US_ID { get; set; }
        public Subscription Subscription { get; set; }
        public bool isActive { get; set; }
        public DateOnly ActivationDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateOnly NextPaymentDate { get; set; }
        public int DaysRemaining { get; set; }
        public Club Club { get; set; }
        public string PaymentMethod { get; set; }
        public string PromoAction { get; set; }
    }
}
