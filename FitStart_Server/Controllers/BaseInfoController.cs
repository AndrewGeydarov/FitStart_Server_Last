using FitStart_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class BaseInfoController : ControllerBase
    {
        private readonly IBaseInfoService _baseInfoService;
        public BaseInfoController(IBaseInfoService baseInfoService)
        {
            _baseInfoService = baseInfoService;
        }

        [HttpGet]
        [Route("/fitstart/clubs")]
        public async Task<IActionResult> GetClubs()
        {
            return await _baseInfoService.GetClubs();
        }

        [HttpGet]
        [Route("/fitstart/club/{ClubID}")]
        public async Task<IActionResult> GetClubById(int ClubID)
        {
            return await _baseInfoService.GetClubById(ClubID);
        }

        [HttpGet]
        [Route("/fitstart/club/{ClubID}/load/{dayOfWeek}")]
        public async Task<IActionResult> GetClubLoad(int ClubID, int dayOfWeek)
        {
            return await _baseInfoService.GetClubLoad(ClubID, dayOfWeek);
        }

        [HttpGet]
        [Route("/fitstart/banners")]
        public async Task<IActionResult> GetActiveBanners()
        {
            return await _baseInfoService.GetActiveBanners();
        }

        [HttpGet]
        [Route("/fitstart/trainercategories")]
        public async Task<IActionResult> GetTrainerCategories()
        {
            return await _baseInfoService.GetTrainerCategories();
        }

        [HttpGet]
        [Route("/fitstart/exercisetypes")]
        public async Task<IActionResult> GetExerciseTypes()
        {
            return await _baseInfoService.GetExerciseTypes();
        }

        [HttpGet]
        [Route("/fitstart/musclegroups")]
        public async Task<IActionResult> GetMuscleGroups()
        {
            return await _baseInfoService.GetMuscleGroups();
        }

        [HttpGet]
        [Route("/fitstart/equipmenttypes")]
        public async Task<IActionResult> GetEquipmentTypes()
        {
            return await _baseInfoService.GetEquipmentTypes();
        }

        [HttpGet]
        [Route("/fitstart/workouttypes")]
        public async Task<IActionResult> GetWorkoutTypes()
        {
            return await _baseInfoService.GetWorkoutTypes();
        }

        [HttpGet]
        [Route("/fitstart/classes")]
        public async Task<IActionResult> GetClasses()
        {
            return await _baseInfoService.GetClasses();
        }
    }
}
