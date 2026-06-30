using FitStart_Server.CustomAttributes;
using FitStart_Server.Interfaces;
using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost]
        [Route("/fitstart/schedule/filter/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetSchedule(int UserID, [FromBody] ScheduleFilterModel filter)
        {
            return await _scheduleService.GetSchedule(UserID, filter);
        }

        [HttpGet]
        [Route("/fitstart/schedule/{UserID}/date/{date}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetScheduleByDate(int UserID, DateOnly date)
        {
            return await _scheduleService.GetScheduleByDate(UserID, date);
        }

        [HttpGet]
        [Route("/fitstart/schedule/item/{ScheduleID}/user/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetScheduleItemById(int ScheduleID, int UserID)
        {
            return await _scheduleService.GetScheduleItemById(ScheduleID, UserID);
        }

        [HttpPost]
        [Route("/fitstart/schedule/signup")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> SignUpForClass([FromBody] SignUpForClassModel model)
        {
            return await _scheduleService.SignUpForClass(model);
        }

        [HttpPost]
        [Route("/fitstart/schedule/cancel")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> CancelClass([FromBody] CancelClassModel model)
        {
            return await _scheduleService.CancelClass(model);
        }

        [HttpGet]
        [Route("/fitstart/schedule/upcoming/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetUserUpcomingTrainings(int UserID)
        {
            return await _scheduleService.GetUserUpcomingTrainings(UserID);
        }
    }
}
