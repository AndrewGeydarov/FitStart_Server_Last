using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> GetHomeScreen(int UserID);
        Task<IActionResult> EditUser(EditUserModel model);
        Task<IActionResult> DeleteUser(int UserID);
        Task<IActionResult> ToggleVibration(ToggleVibrationModel model);
        Task<IActionResult> TopUpBalance(TopUpBalanceModel model);
        Task<IActionResult> ConfirmTopUp(ConfirmTopUpModel model);
        Task<IActionResult> GetBalance(int UserID);
        Task<IActionResult> GetPaymentHistory(int UserID);
        Task<IActionResult> GeneratePass(int UserID);
        Task<IActionResult> GetNotifications(int UserID);
        Task<IActionResult> MarkNotificationAsRead(int notificationID);
        Task<IActionResult> AddBodyComposition(AddBodyCompositionModel model);
        Task<IActionResult> GetBodyCompositionHistory(int UserID);
        Task<IActionResult> GetTrainingDiary(int UserID);
    }
}
