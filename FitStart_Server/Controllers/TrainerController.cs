using FitStart_Server.CustomAttributes;
using FitStart_Server.Interfaces;
using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class TrainerController : ControllerBase
    {
        private readonly ITrainerService _trainerService;
        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        [HttpGet]
        [Route("/fitstart/trainers")]
        public async Task<IActionResult> GetTrainers([FromQuery] int? clubID, [FromQuery] int? categoryID)
        {
            return await _trainerService.GetTrainers(clubID, categoryID);
        }

        [HttpGet]
        [Route("/fitstart/trainer/{TrainerID}")]
        public async Task<IActionResult> GetTrainerById(int TrainerID)
        {
            return await _trainerService.GetTrainerById(TrainerID);
        }

        [HttpGet]
        [Route("/fitstart/trainer/{TrainerID}/responses")]
        public async Task<IActionResult> GetTrainerResponses(int TrainerID)
        {
            return await _trainerService.GetTrainerResponses(TrainerID);
        }

        [HttpPost]
        [Route("/fitstart/trainer/response/add")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> AddResponse([FromBody] AddResponseModel model)
        {
            return await _trainerService.AddResponse(model);
        }

        [HttpPost]
        [Route("/fitstart/trainer/booking/add")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> BookPersonalTraining([FromBody] PersonalTrainingModel model)
        {
            return await _trainerService.BookPersonalTraining(model);
        }

        [HttpDelete]
        [Route("/fitstart/trainer/booking/cancel/{personalTrainingID}/user/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> CancelPersonalTraining(int personalTrainingID, int UserID)
        {
            return await _trainerService.CancelPersonalTraining(personalTrainingID, UserID);
        }

        [HttpGet]
        [Route("/fitstart/trainer/bookings/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetUserPersonalTrainings(int UserID)
        {
            return await _trainerService.GetUserPersonalTrainings(UserID);
        }
    }
}
