using FitStart_Server.Models;

namespace FitStart_Server.ReturnModels
{
    public class ReturnTrainerModel
    {
        public int TrainerID { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public double HourCost { get; set; }
        public int WorkExperience { get; set; }
        public string AboutTrainer { get; set; }
        public string PhotoPath { get; set; }
        public TrainerCategory TrainerCategory { get; set; }
        public Club Club { get; set; }
        public double AverageRating { get; set; }
        public int ResponseCount { get; set; }
    }
}
