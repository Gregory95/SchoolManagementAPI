using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementAPI.ViewModels
{
    public class BaseViewModel
    {
        public dynamic? Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string? ModifiedBy { get; set; } = null;
        public string? CreatedBy { get; set; } = null;
    }
}
