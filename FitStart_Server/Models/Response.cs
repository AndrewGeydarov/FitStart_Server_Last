using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Response
    {
        [Key]
        public int ResponseID { get; set; }
        public int ResponseRate { get; set; }
        public string ResponseContent { get; set; }
        public DateOnly ResponseDate { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Trainer))]
        public int TrainerID { get; set; }
        public Trainer Trainer { get; set; }
    }
}
