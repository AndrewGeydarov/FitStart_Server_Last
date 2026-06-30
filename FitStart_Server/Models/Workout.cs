using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Workout
    {
        [Key]
        public int WorkoutID { get; set; }
        public string WorkoutName { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int Intensity { get; set; }
        public string PreviewPath { get; set; }
        public string VideoPath { get; set; }
        public string TrainerName { get; set; }
        public bool isTrial { get; set; }
        [ForeignKey(nameof(Workout_Type))]
        public int WT_ID { get; set; }
        public Workout_Type Workout_Type { get; set; }
    }
}
