using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class Club
    {
        [Key]
        public int ClubID { get; set; }
        public string ClubName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public string PhotoPath { get; set; }
    }
}
