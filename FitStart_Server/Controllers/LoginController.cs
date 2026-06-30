using FitStart_Server.CustomAttributes;
using FitStart_Server.Interfaces;
using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        [Route("/fitstart/signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            return await _loginService.SignUp(model);
        }

        [HttpPost]
        [Route("/fitstart/login")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel model)
        {
            return await _loginService.SignIn(model);
        }

        [HttpGet]
        [Route("/fitstart/profile/{UserID}")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> GetProfileInfo(int UserID)
        {
            return await _loginService.GetProfileInfo(UserID);
        }

        [HttpPost]
        [Route("/fitstart/logout/{UserID}")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> Logout(int UserID)
        {
            var header = Request.Headers["Authorization"].ToString();
            string token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? header.Substring("Bearer ".Length).Trim()
                : header.Trim();
            return await _loginService.Logout(UserID, token);
        }
    }
}
