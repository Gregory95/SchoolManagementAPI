using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementAPI.ViewModels.Role
{
    public class RoleVM : BaseViewModel
    {
        [MaxLength(55)]
        public string? Id { get; set; }
        [MaxLength(255)]
        public string Discriminator { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public long CurrencyStamp { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
