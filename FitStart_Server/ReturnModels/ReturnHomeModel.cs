using FitStart_Server.Models;

namespace FitStart_Server.ReturnModels
{
    public class ReturnHomeModel
    {
        public User User { get; set; }
        public ReturnSubscriptionInfoModel ActiveSubscription { get; set; }
        public Banner[] Banners { get; set; }
        public int UnreadNotifications { get; set; }
        public ReturnScheduleItemModel[] UpcomingTrainings { get; set; }
    }
}
