using FitStart_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        public ExerciseController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet]
        [Route("/fitstart/exercises")]
        public async Task<IActionResult> GetExercises([FromQuery] int? typeID, [FromQuery] int? muscleGroupID)
        {
            return await _exerciseService.GetExercises(typeID, muscleGroupID);
        }

        [HttpGet]
        [Route("/fitstart/exercise/{ExerciseID}")]
        public async Task<IActionResult> GetExerciseById(int ExerciseID)
        {
            return await _exerciseService.GetExerciseById(ExerciseID);
        }
    }
}
