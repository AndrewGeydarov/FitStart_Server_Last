using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class Banner
    {
        [Key]
        public int BannerID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ActionLink { get; set; }
        public bool isActive { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
