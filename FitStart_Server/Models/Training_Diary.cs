using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Training_Diary
    {
        [Key]
        public int TrainingDiaryID { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Schedule))]
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }
        public DateTime SignUpDate { get; set; }
        public bool isAttended { get; set; }
        public bool isCancelled { get; set; }
    }
}
