using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Trainer
    {
        [Key]
        public int TrainerID { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public double HourCost { get; set; }
        public int WorkExperience { get; set; }
        public string AboutTrainer { get; set; }
        public string PhotoPath { get; set; }
        [ForeignKey(nameof(TrainerCategory))]
        public int TrainerCategoryID { get; set; }
        public TrainerCategory TrainerCategory { get; set; }
        [ForeignKey(nameof(Club))]
        public int ClubID { get; set; }
        public Club Club { get; set; }
    }
}
