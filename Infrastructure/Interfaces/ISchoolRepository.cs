using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.ViewModels.Schools;
using SchoolManagementAPI.ViewModels.Application;
using System.Security.Policy;

namespace SchoolManagementAPI.Interfaces
{
    public interface ISchoolRepository
    {
        public Task<SchoolVM> GetSchoolById(int id);
        public Task<List<SchoolVM>> GetAllSchools();
        public Task<SchoolVM> AddSchool(SchoolAddVM model);
        public Task<SchoolVM> EditSchool(SchoolEditVM model);
        public Task<int> DeleteSchool(int id);
    }
}
