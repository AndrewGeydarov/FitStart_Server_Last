using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class Workout_Type
    {
        [Key]
        public int WT_ID { get; set; }
        public string TypeName { get; set; }
        public string Category { get; set; }
    }
}
