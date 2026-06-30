using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitStart_Server.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string? Patronymic { get; set; }
        public DateOnly BirthDate { get; set; }
        public string AvatarPath { get; set; }
        public double Balance { get; set; }
        public bool VibrationOnStart { get; set; }
        [ForeignKey(nameof(Login))]
        public int LoginID { get; set; }
        public Login Login { get; set; }
        [ForeignKey(nameof(Role))]
        public int RoleID { get; set; }
        public Role Role { get; set; }
    }
}
