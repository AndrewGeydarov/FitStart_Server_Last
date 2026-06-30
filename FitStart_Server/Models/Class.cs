using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class Class
    {
        [Key]
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int Intensity { get; set; }
    }
}
