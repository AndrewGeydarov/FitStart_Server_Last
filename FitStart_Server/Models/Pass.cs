using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Pass
    {
        [Key]
        public int PassID { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        public string QRCode { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool isUsed { get; set; }
    }
}
