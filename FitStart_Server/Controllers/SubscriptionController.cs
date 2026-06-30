using FitStart_Server.CustomAttributes;
using FitStart_Server.Interfaces;
using FitStart_Server.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        [Route("/fitstart/subscriptions/available")]
        public async Task<IActionResult> GetAvailableSubscriptions()
        {
            return await _subscriptionService.GetAvailableSubscriptions();
        }

        [HttpPost]
        [Route("/fitstart/subscription/purchase")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> PurchaseSubscription([FromBody] PurchaseSubscriptionModel model)
        {
            return await _subscriptionService.PurchaseSubscription(model);
        }

        [HttpGet]
        [Route("/fitstart/subscription/active/{UserID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetUserActiveSubscription(int UserID)
        {
            return await _subscriptionService.GetUserActiveSubscription(UserID);
        }

        [HttpPost]
        [Route("/fitstart/subscription/pay/{US_ID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> PaySubscription(int US_ID)
        {
            return await _subscriptionService.PaySubscription(US_ID);
        }

        [HttpPost]
        [Route("/fitstart/subscription/freeze")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> FreezeSubscription([FromBody] FreezeSubscriptionModel model)
        {
            return await _subscriptionService.FreezeSubscription(model);
        }

        [HttpPost]
        [Route("/fitstart/subscription/unfreeze/{FreezeID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> UnfreezeSubscription(int FreezeID)
        {
            return await _subscriptionService.UnfreezeSubscription(FreezeID);
        }

        [HttpGet]
        [Route("/fitstart/subscription/freezes/{US_ID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> GetFreezeHistory(int US_ID)
        {
            return await _subscriptionService.GetFreezeHistory(US_ID);
        }

        [HttpPut]
        [Route("/fitstart/subscription/changeclub")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> ChangeClub([FromBody] ChangeClubModel model)
        {
            return await _subscriptionService.ChangeClub(model);
        }

        [HttpPut]
        [Route("/fitstart/subscription/changepayment")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> ChangePaymentMethod([FromBody] ChangePaymentMethodModel model)
        {
            return await _subscriptionService.ChangePaymentMethod(model);
        }

        [HttpPost]
        [Route("/fitstart/subscription/cancel/{US_ID}")]
        [RoleAuthorize([2])]
        public async Task<IActionResult> CancelSubscription(int US_ID)
        {
            return await _subscriptionService.CancelSubscription(US_ID);
        }
    }
}
