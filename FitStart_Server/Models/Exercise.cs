using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Exercise
    {
        [Key]
        public int ExerciseID { get; set; }
        public string ExerciseName { get; set; }
        public string Difficulty { get; set; }
        public string Technique { get; set; }
        public string Recommendations { get; set; }
        public string PhotoPath { get; set; }
        public string VideoPath { get; set; }
        [ForeignKey(nameof(Exercise_Type))]
        public int ET_ID { get; set; }
        public Exercise_Type Exercise_Type { get; set; }
    }
}
