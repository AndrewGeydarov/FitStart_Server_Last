using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Subscription_Freeze
    {
        [Key]
        public int FreezeID { get; set; }
        [ForeignKey(nameof(User_Subscription))]
        public int US_ID { get; set; }
        public User_Subscription User_Subscription { get; set; }
        public DateOnly FreezeStart { get; set; }
        public DateOnly FreezeEnd { get; set; }
        public int FreezeDays { get; set; }
        public bool isActive { get; set; }
    }
}
