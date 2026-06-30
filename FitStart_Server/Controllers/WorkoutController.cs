using FitStart_Server.CustomAttributes;
using FitStart_Server.Interfaces;
using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;
        public WorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet]
        [Route("/fitstart/workouts/user/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetWorkouts(int UserID, [FromQuery] int? typeID, [FromQuery] string? category, [FromQuery] bool? onlyTrial, [FromQuery] bool? onlyFavorite)
        {
            return await _workoutService.GetWorkouts(UserID, typeID, category, onlyTrial, onlyFavorite);
        }

        [HttpGet]
        [Route("/fitstart/workout/{WorkoutID}/user/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetWorkoutById(int WorkoutID, int UserID)
        {
            return await _workoutService.GetWorkoutById(WorkoutID, UserID);
        }

        [HttpPost]
        [Route("/fitstart/workout/favorite/add")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteWorkoutModel model)
        {
            return await _workoutService.AddToFavorites(model);
        }

        [HttpDelete]
        [Route("/fitstart/workout/favorite/remove")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> RemoveFromFavorites([FromBody] FavoriteWorkoutModel model)
        {
            return await _workoutService.RemoveFromFavorites(model);
        }

        [HttpGet]
        [Route("/fitstart/workouts/favorites/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetUserFavorites(int UserID)
        {
            return await _workoutService.GetUserFavorites(UserID);
        }
    }
}
