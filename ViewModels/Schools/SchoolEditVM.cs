using System.ComponentModel.DataAnnotations;

namespace SchoolManagementAPI.ViewModels.Schools
{
    public class SchoolEditVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Region { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
        [MaxLength(255)]
        public string LandLine1 { get; set; }
        [MaxLength(255)]
        public string LandLine2 { get; set; }
        public int? NumberOfStudents { get; set; } = 0;
        public int? NumberOfTeachers { get; set; } = 0;
    }
}
