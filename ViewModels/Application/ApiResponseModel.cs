namespace SchoolManagementAPI.ViewModels.Application
{
    public class ApiResponseModel
    {
        public dynamic? Response { get; set; }
        public ICollection<ErrorList>? ErrorList { get; set; }
    }
}
