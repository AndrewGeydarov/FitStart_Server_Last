using FitStart_Server.CustomAttributes;
using FitStart_Server.Interfaces;
using FitStart_Server.Requests;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [Route("/fitstart/avatar/add")]
        [RoleAuthorize([1, 2])]
        public async Task<ReturnFileResult> AddAvatar([FromForm] AddAvatarModel model)
        {
            return await _fileService.AddAvatar(model);
        }

        [HttpGet]
        [Route("/fitstart/avatar/download/{UserID}")]
        public async Task<IActionResult> DownloadAvatar(int UserID)
        {
            return await _fileService.DownloadAvatar(UserID);
        }

        [HttpDelete]
        [Route("/fitstart/avatar/delete/{UserID}")]
        [RoleAuthorize([1, 2])]
        public async Task<ReturnFileResult> DeleteAvatar(int UserID)
        {
            return await _fileService.DeleteAvatar(UserID);
        }

        [HttpGet]
        [Route("/fitstart/trainer/photo/{TrainerID}")]
        public async Task<IActionResult> DownloadTrainerPhoto(int TrainerID)
        {
            return await _fileService.DownloadTrainerPhoto(TrainerID);
        }

        [HttpGet]
        [Route("/fitstart/equipment/photo/{EquipmentID}")]
        public async Task<IActionResult> DownloadEquipmentPhoto(int EquipmentID)
        {
            return await _fileService.DownloadEquipmentPhoto(EquipmentID);
        }

        [HttpGet]
        [Route("/fitstart/exercise/photo/{ExerciseID}")]
        public async Task<IActionResult> DownloadExercisePhoto(int ExerciseID)
        {
            return await _fileService.DownloadExercisePhoto(ExerciseID);
        }

        [HttpGet]
        [Route("/fitstart/workout/preview/{WorkoutID}")]
        public async Task<IActionResult> DownloadWorkoutPreview(int WorkoutID)
        {
            return await _fileService.DownloadWorkoutPreview(WorkoutID);
        }

        [HttpGet]
        [Route("/fitstart/banner/image/{BannerID}")]
        public async Task<IActionResult> DownloadBannerImage(int BannerID)
        {
            return await _fileService.DownloadBannerImage(BannerID);
        }

        [HttpGet]
        [Route("/fitstart/club/photo/{ClubID}")]
        public async Task<IActionResult> DownloadClubPhoto(int ClubID)
        {
            return await _fileService.DownloadClubPhoto(ClubID);
        }
    }
}
