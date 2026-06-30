using FitStart_Server.Models;

namespace FitStart_Server.ReturnModels
{
    public class ReturnEquipmentModel
    {
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; }
        public string Description { get; set; }
        public string Instruction { get; set; }
        public string PhotoPath { get; set; }
        public string VideoPath { get; set; }
        public bool isAvailable { get; set; }
        public string Location { get; set; }
        public TrainingEquip_Type Type { get; set; }
        public Club Club { get; set; }
    }
}
