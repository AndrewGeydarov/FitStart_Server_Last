using FitStart_Server.Requests;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface IFileService
    {
        Task<ReturnFileResult> AddAvatar(AddAvatarModel model);
        Task<IActionResult> DownloadAvatar(int UserID);
        Task<ReturnFileResult> DeleteAvatar(int UserID);
        Task<IActionResult> DownloadTrainerPhoto(int TrainerID);
        Task<IActionResult> DownloadEquipmentPhoto(int EquipmentID);
        Task<IActionResult> DownloadExercisePhoto(int ExerciseID);
        Task<IActionResult> DownloadWorkoutPreview(int WorkoutID);
        Task<IActionResult> DownloadBannerImage(int BannerID);
        Task<IActionResult> DownloadClubPhoto(int ClubID);
    }
}
