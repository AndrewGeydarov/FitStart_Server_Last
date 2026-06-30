using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface ITrainerService
    {
        Task<IActionResult> GetTrainers(int? clubID, int? categoryID);
        Task<IActionResult> GetTrainerById(int TrainerID);
        Task<IActionResult> GetTrainerResponses(int TrainerID);
        Task<IActionResult> AddResponse(AddResponseModel model);
        Task<IActionResult> BookPersonalTraining(PersonalTrainingModel model);
        Task<IActionResult> CancelPersonalTraining(int personalTrainingID, int UserID);
        Task<IActionResult> GetUserPersonalTrainings(int UserID);
    }
}
