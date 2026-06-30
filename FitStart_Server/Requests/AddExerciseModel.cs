namespace FitStart_Server.Requests
{
    public class AddExerciseModel
    {
        public string ExerciseName { get; set; }
        public string Difficulty { get; set; }
        public string Technique { get; set; }
        public string Recommendations { get; set; }
        public int ET_ID { get; set; }
        public int[] MuscleGroupIDs { get; set; }
    }
}
