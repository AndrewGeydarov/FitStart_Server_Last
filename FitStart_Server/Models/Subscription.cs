using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class Subscription
    {
        [Key]
        public int SubID { get; set; }
        public string SubscriptionName { get; set; }
        public double Cost { get; set; }
        public int DurationDays { get; set; }
        public string Description { get; set; }
        public double EntryFee { get; set; }
    }
}
