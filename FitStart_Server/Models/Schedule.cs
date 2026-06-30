using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleID { get; set; }
        [ForeignKey(nameof(Trainer))]
        public int TrainerID { get; set; }
        public Trainer Trainer { get; set; }
        [ForeignKey(nameof(Class))]
        public int ClassID { get; set; }
        public Class Class { get; set; }
        [ForeignKey(nameof(Club))]
        public int ClubID { get; set; }
        public Club Club { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int MaxSlots { get; set; }
    }
}
