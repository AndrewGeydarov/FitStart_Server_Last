using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isRead { get; set; }
    }
}
