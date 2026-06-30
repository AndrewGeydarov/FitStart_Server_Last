using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Personal_Training
    {
        [Key]
        public int PersonalTrainingID { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Trainer))]
        public int TrainerID { get; set; }
        public Trainer Trainer { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public double Cost { get; set; }
        public bool isCancelled { get; set; }
    }
}
