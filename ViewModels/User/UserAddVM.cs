using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementAPI.ViewModels.User
{
    public class UserAddVM
    {
        public int? Id { get; set; }
        [MaxLength(255)]
        public string UserName { get; set; }
        [NotMapped]
        [MaxLength(2000)]
        public string Password { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(255)]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(255)]
        public string Role { get; set; }
    }
}
