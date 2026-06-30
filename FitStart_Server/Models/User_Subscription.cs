using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class User_Subscription
    {
        [Key]
        public int US_ID { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Subscription))]
        public int SubscriptionID { get; set; }
        public Subscription Subscription { get; set; }
        public DateTime SubDate { get; set; }
        public bool isActive { get; set; }
        public DateOnly ActivationDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateOnly NextPaymentDate { get; set; }
        [ForeignKey(nameof(Club))]
        public int ClubID { get; set; }
        public Club Club { get; set; }
        public string PaymentMethod { get; set; }
        public string PromoAction { get; set; }
    }
}
