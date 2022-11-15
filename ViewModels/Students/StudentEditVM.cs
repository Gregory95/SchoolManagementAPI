using System.ComponentModel.DataAnnotations;

namespace SchoolManagementAPI.ViewModels.Students
{
    public class StudentEditVM
    {
        public int Id { get; set; }
        [MaxLength(255)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(255)]
        [Required]
        public string LastName { get; set; }
        [MaxLength(255)]
        [Required]
        public string IdentificationNumber { get; set; }
        [MaxLength(255)]
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(255)]
        public string? MobileNumber { get; set; }
        public int? SchoolId { get; set; }
    }
}
