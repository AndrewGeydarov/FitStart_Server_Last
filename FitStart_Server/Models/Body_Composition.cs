using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class Body_Composition
    {
        [Key]
        public int BodyCompositionID { get; set; }
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public User User { get; set; }
        public DateOnly MeasureDate { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BodyFatPercent { get; set; }
        public double MuscleMass { get; set; }
        public double WaterPercent { get; set; }
        public double BMI { get; set; }
    }
}
