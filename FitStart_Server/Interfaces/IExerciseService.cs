using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface IExerciseService
    {
        Task<IActionResult> GetExercises(int? typeID, int? muscleGroupID);
        Task<IActionResult> GetExerciseById(int ExerciseID);
    }
}
