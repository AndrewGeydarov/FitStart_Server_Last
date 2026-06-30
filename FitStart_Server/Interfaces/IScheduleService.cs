using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface IScheduleService
    {
        Task<IActionResult> GetSchedule(int UserID, ScheduleFilterModel filter);
        Task<IActionResult> GetScheduleByDate(int UserID, DateOnly date);
        Task<IActionResult> GetScheduleItemById(int ScheduleID, int UserID);
        Task<IActionResult> SignUpForClass(SignUpForClassModel model);
        Task<IActionResult> CancelClass(CancelClassModel model);
        Task<IActionResult> GetUserUpcomingTrainings(int UserID);
    }
}
