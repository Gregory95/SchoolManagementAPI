using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementAPI.Models.Student
{
    public class Student : BaseModel
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(255)]
        public string IdentificationNumber { get; set; }
        [MaxLength(255)]
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(255)]
        public string? MobileNumber { get; set; }
        public int? SchoolId { get; set; }
        [ForeignKey("SchoolId")]
        public SchoolManagementAPI.Models.School.School? School { get; set; }
    }
}
