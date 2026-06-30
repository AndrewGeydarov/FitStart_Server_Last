using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class TrainerCategory
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
