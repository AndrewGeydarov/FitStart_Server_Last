using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface IWorkoutService
    {
        Task<IActionResult> GetWorkouts(int UserID, int? typeID, string? category, bool? onlyTrial, bool? onlyFavorite);
        Task<IActionResult> GetWorkoutById(int WorkoutID, int UserID);
        Task<IActionResult> AddToFavorites(FavoriteWorkoutModel model);
        Task<IActionResult> RemoveFromFavorites(FavoriteWorkoutModel model);
        Task<IActionResult> GetUserFavorites(int UserID);
    }
}
