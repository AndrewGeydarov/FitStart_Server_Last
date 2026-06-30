namespace FitStart_Server.Requests
{
    public class EditTrainerModel
    {
        public int TrainerID { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public double? HourCost { get; set; }
        public int? WorkExperience { get; set; }
        public string AboutTrainer { get; set; }
        public int? TrainerCategoryID { get; set; }
        public int? ClubID { get; set; }
    }
}
