using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class Exercise_Type
    {
        [Key]
        public int ET_ID { get; set; }
        public string TypeName { get; set; }
    }
}
