using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.ViewModels.Application;
using SchoolManagementAPI.ViewModels.Students;
using System.Security.Policy;

namespace SchoolManagementAPI.Interfaces
{
    public interface IStudentRepository
    {
        public Task<StudentVM> GetStudentById(int id);
        public Task<List<StudentVM>> GetAllStudents();
        public Task<StudentVM> AddStudent(StudentAddVM model);
        public Task<StudentVM> EditStudent(StudentEditVM model);
        public Task<int> DeleteStudent(int id);
    }
}
