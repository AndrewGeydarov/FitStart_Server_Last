using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Exercise_Description
    {
        [Key]
        public int DescriptionID { get; set; }
        [ForeignKey(nameof(Exercise))]
        public int ExerciseID { get; set; }
        public Exercise Exercise { get; set; }
        [ForeignKey(nameof(Exercise_Type))]
        public int ET_ID { get; set; }
        public Exercise_Type Exercise_Type { get; set; }
        [ForeignKey(nameof(Muscle_Group))]
        public int MG_ID { get; set; }
        public Muscle_Group Muscle_Group { get; set; }
    }
}
