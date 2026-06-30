using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Session
    {
        [Key]
        public int SessionID { get; set; }
        public string Token { get; set; }
        public DateTime LogedAt { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
