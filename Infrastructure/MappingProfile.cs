global using AutoMapper;
using SchoolManagementAPI.Models.Student;
using SchoolManagementAPI.Models.School;
using SchoolManagementAPI.ViewModels.Students;
using SchoolManagementAPI.ViewModels.Schools;
using SchoolManagementAPI.Models.User;
using SchoolManagementAPI.ViewModels.User;
using SchoolManagementAPI.ViewModels.Role;

namespace SchoolManagementAPI.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            //* Users
            CreateMap<UserVM, ApplicationUser>();
            CreateMap<ApplicationUser, UserVM>();

            //* Roles
            CreateMap<RoleVM, ApplicationRole>();
            CreateMap<ApplicationRole, RoleVM>();

            //* Students
            CreateMap<Student, StudentVM>();
            CreateMap<StudentVM, Student>();

            CreateMap<Student, StudentAddVM>();
            CreateMap<StudentAddVM, Student>();

            CreateMap<Student, StudentEditVM>();
            CreateMap<StudentEditVM, Student>();

            CreateMap<StudentVM, StudentEditVM>();
            CreateMap<StudentEditVM, StudentVM>();

            //* Schools
            CreateMap<School, SchoolVM>();
            CreateMap<SchoolVM, School>();

            CreateMap<School, SchoolAddVM>();
            CreateMap<SchoolAddVM, School>();

            CreateMap<School, SchoolEditVM>();
            CreateMap<SchoolEditVM, School>();

            CreateMap<SchoolVM, SchoolEditVM>();
            CreateMap<SchoolEditVM, SchoolVM>();
        }
    }
}
