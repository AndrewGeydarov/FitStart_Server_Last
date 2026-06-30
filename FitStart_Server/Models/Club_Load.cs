using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Club_Load
    {
        [Key]
        public int ClubLoadID { get; set; }
        [ForeignKey(nameof(Club))]
        public int ClubID { get; set; }
        public Club Club { get; set; }
        public int DayOfWeek { get; set; }
        public int Hour { get; set; }
        public int LoadPercent { get; set; }
    }
}
