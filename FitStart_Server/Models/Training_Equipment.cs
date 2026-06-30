using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Training_Equipment
    {
        [Key]
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string Description { get; set; }
        public string Instruction { get; set; }
        public string PhotoPath { get; set; }
        public string VideoPath { get; set; }
        public bool isAvailable { get; set; }
        public string Location { get; set; }
        [ForeignKey(nameof(TrainingEquip_Type))]
        public int TET_ID { get; set; }
        public TrainingEquip_Type TrainingEquip_Type { get; set; }
        [ForeignKey(nameof(Club))]
        public int ClubID { get; set; }
        public Club Club { get; set; }
    }
}
