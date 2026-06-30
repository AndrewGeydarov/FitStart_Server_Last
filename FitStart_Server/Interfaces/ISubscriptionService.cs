using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IActionResult> GetAvailableSubscriptions();
        Task<IActionResult> PurchaseSubscription(PurchaseSubscriptionModel model);
        Task<IActionResult> GetUserActiveSubscription(int UserID);
        Task<IActionResult> PaySubscription(int US_ID);
        Task<IActionResult> FreezeSubscription(FreezeSubscriptionModel model);
        Task<IActionResult> UnfreezeSubscription(int FreezeID);
        Task<IActionResult> GetFreezeHistory(int US_ID);
        Task<IActionResult> ChangeClub(ChangeClubModel model);
        Task<IActionResult> ChangePaymentMethod(ChangePaymentMethodModel model);
        Task<IActionResult> CancelSubscription(int US_ID);
    }
}
