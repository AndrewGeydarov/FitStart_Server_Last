using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class TrainingEquip_Type
    {
        [Key]
        public int TET_ID { get; set; }
        public string TypeName { get; set; }
        public string Zone { get; set; }
    }
}
