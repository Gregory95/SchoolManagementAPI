using System.ComponentModel.DataAnnotations;

namespace SchoolManagementAPI.ViewModels.Students
{
    public class StudentVM
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        [MaxLength(255)]
        public string FirstName { get; set; }
        [MaxLength(255)]
        public string LastName { get; set; }
        [MaxLength(255)]
        public string IdentificationNumber { get; set; }
        [MaxLength(255)]
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(255)]
        public string? MobileNumber { get; set; }
        public int? SchoolId { get; set; }
        [MaxLength(255)]
        public string? SchoolName { get; set; }
    }
}
