using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementAPI.Models.School
{
    public class School : BaseModel
    {
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
