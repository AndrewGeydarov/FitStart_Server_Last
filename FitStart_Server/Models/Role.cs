using System.ComponentModel.DataAnnotations;

namespace FitStart_Server.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
