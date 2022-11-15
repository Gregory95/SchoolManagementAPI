using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using SchoolManagementAPI.Models.User;

namespace SchoolManagementAPI.ViewModels.User
{
    public class UserVM : BaseViewModel
    {
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? PasswordModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool HasFirstLogin { get; set; } = false;
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
