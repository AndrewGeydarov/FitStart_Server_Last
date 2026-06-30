using FitStart_Server.Models;

namespace FitStart_Server.ReturnModels
{
    public class ReturnExerciseModel
    {
        public int ExerciseID { get; set; }
        public string ExerciseName { get; set; }
        public string Difficulty { get; set; }
        public string Technique { get; set; }
        public string Recommendations { get; set; }
        public string PhotoPath { get; set; }
        public string VideoPath { get; set; }
        public Exercise_Type ExerciseType { get; set; }
        public Muscle_Group[] MuscleGroups { get; set; }
    }
}
