using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class User_Favorite_Workout
    {
        [Key]
        public int UFW_ID { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Workout))]
        public int WorkoutID { get; set; }
        public Workout Workout { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
