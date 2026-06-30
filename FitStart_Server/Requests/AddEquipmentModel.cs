namespace FitStart_Server.Requests
{
    public class AddEquipmentModel
    {
        public string EquipmentName { get; set; }
        public string Description { get; set; }
        public string Instruction { get; set; }
        public string Location { get; set; }
        public int TET_ID { get; set; }
        public int ClubID { get; set; }
    }
}
