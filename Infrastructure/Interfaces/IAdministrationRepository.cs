using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.ViewModels.Application;
using System.Security.Policy;
using SchoolManagementAPI.ViewModels.Role;

namespace SchoolManagementAPI.Interfaces
{
    public interface IAdministrationRepository
    {
        public Task<RoleVM> GetRoleById(string id);
        public Task<List<RoleVM>> GetAllRoles();
        public Task<RoleVM> AddRole(RoleVM model);
        public Task<RoleVM> EditRole(RoleVM model);
        public Task<string> DeleteRole(string id);
    }
}
