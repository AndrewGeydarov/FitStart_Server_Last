using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class Muscle_Group
    {
        [Key]
        public int MG_ID { get; set; }
        public string MuscleGroupName { get; set; }
    }
}
