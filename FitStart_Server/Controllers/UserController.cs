using FitStart_Server.CustomAttributes;
using FitStart_Server.Interfaces;
using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("/fitstart/user/home/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetHomeScreen(int UserID)
        {
            return await _userService.GetHomeScreen(UserID);
        }

        [HttpPut]
        [Route("/fitstart/user/edit")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> EditUser([FromBody] EditUserModel model)
        {
            return await _userService.EditUser(model);
        }

        [HttpDelete]
        [Route("/fitstart/user/delete/{UserID}")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> DeleteUser(int UserID)
        {
            return await _userService.DeleteUser(UserID);
        }

        [HttpPut]
        [Route("/fitstart/user/togglevibration")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> ToggleVibration([FromBody] ToggleVibrationModel model)
        {
            return await _userService.ToggleVibration(model);
        }

        [HttpPost]
        [Route("/fitstart/user/topup")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> TopUpBalance([FromBody] TopUpBalanceModel model)
        {
            return await _userService.TopUpBalance(model);
        }

        [HttpPost]
        [Route("/fitstart/user/topup/confirm")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> ConfirmTopUp([FromBody] ConfirmTopUpModel model)
        {
            return await _userService.ConfirmTopUp(model);
        }

        [HttpGet]
        [Route("/fitstart/user/balance/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetBalance(int UserID)
        {
            return await _userService.GetBalance(UserID);
        }

        [HttpGet]
        [Route("/fitstart/user/payments/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetPaymentHistory(int UserID)
        {
            return await _userService.GetPaymentHistory(UserID);
        }

        [HttpPost]
        [Route("/fitstart/user/generatepass/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GeneratePass(int UserID)
        {
            return await _userService.GeneratePass(UserID);
        }

        [HttpGet]
        [Route("/fitstart/user/notifications/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetNotifications(int UserID)
        {
            return await _userService.GetNotifications(UserID);
        }

        [HttpPut]
        [Route("/fitstart/user/notification/read/{notificationID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationID)
        {
            return await _userService.MarkNotificationAsRead(notificationID);
        }

        [HttpPost]
        [Route("/fitstart/user/bodycomposition")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> AddBodyComposition([FromBody] AddBodyCompositionModel model)
        {
            return await _userService.AddBodyComposition(model);
        }

        [HttpGet]
        [Route("/fitstart/user/bodycomposition/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetBodyCompositionHistory(int UserID)
        {
            return await _userService.GetBodyCompositionHistory(UserID);
        }

        [HttpGet]
        [Route("/fitstart/user/diary/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetTrainingDiary(int UserID)
        {
            return await _userService.GetTrainingDiary(UserID);
        }
    }
}
