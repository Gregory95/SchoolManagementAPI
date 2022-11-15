using System.ComponentModel.DataAnnotations;

namespace SchoolManagementAPI.ViewModels.Students
{
    public class StudentsListVM
    {
        public ICollection<StudentVM> Students { get; set; }
    }
}
