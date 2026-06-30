using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface ILoginService
    {
        Task<IActionResult> SignUp(SignUpModel model);
        Task<IActionResult> SignIn(SignInModel model);
        Task<IActionResult> GetProfileInfo(int UserID);
        Task<IActionResult> Logout(int UserID, string token);
    }
}
