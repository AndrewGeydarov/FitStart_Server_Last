using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface IBaseInfoService
    {
        Task<IActionResult> GetClubs();
        Task<IActionResult> GetClubById(int ClubID);
        Task<IActionResult> GetClubLoad(int ClubID, int dayOfWeek);
        Task<IActionResult> GetActiveBanners();
        Task<IActionResult> GetTrainerCategories();
        Task<IActionResult> GetExerciseTypes();
        Task<IActionResult> GetMuscleGroups();
        Task<IActionResult> GetEquipmentTypes();
        Task<IActionResult> GetWorkoutTypes();
        Task<IActionResult> GetClasses();
    }
}
