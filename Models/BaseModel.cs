
namespace SchoolManagementAPI.Models
{
    public class BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; } = null;
        public string CreatedBy { get; set; } = null;
    }
}
