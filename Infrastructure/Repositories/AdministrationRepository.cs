using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Interfaces;
using SchoolManagementAPI.Models.School;
using SchoolManagementAPI.Models.User;
using SchoolManagementAPI.ViewModels.Application;
using SchoolManagementAPI.ViewModels.Role;
using SchoolManagementAPI.ViewModels.User;

namespace SchoolManagementAPI.Infrastructure.Repositories
{
    public class AdministrationRepository : IAdministrationRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdministrationRepository(ApplicationDbContext db
            , IMapper mapper
            , RoleManager<ApplicationRole> roleManager)
        {
            _db = db;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<RoleVM> AddRole(RoleVM model)
        {
            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                throw new Exception("Bad Request, There is already a role with the same name.  Please make sure new role is unique.");
            }

            var newRole = new ApplicationRole
            {
                Name = model.Name,
                NormalizedName = model.Name.ToUpper(),
                Description = model.Description,
                IsDeleted = false,
                IsAdmin = model.IsAdmin,
                Created = DateTime.Now
            };

            await _db.AddAsync(newRole);
            await _db.SaveChangesAsync();

            return _mapper.Map<ApplicationRole, RoleVM>(newRole);
        }

        public async Task<String> DeleteRole(string id)
        {
            var existingRole = await _roleManager.Roles.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (existingRole == null)
            {
                throw new Exception("Role does not exist");
            }

            _db.Remove(existingRole);
            await _db.SaveChangesAsync();

            return id;
        }

        public async Task<RoleVM> EditRole(RoleVM model)
        {
            var existingRole = await _roleManager.Roles.Where(x => x.Name == model.Name).FirstOrDefaultAsync();

            if (existingRole == null)
            {
                throw new Exception("Role does not exist");
            }

            existingRole.ModifiedDate = DateTime.UtcNow;
            existingRole.Name = model.Name;
            existingRole.Description = model.Description;
            existingRole.IsAdmin = model.IsAdmin;
            existingRole.Name = model.Name;
            existingRole.IsDeleted = model.IsDeleted;

            await _db.SaveChangesAsync();

            return _mapper.Map<ApplicationRole, RoleVM>(existingRole);
        }

        public async Task<List<RoleVM>> GetAllRoles()
        {
            return await _roleManager.Roles
                .Select(x => new RoleVM
                {
                    Id = x.Id,
                    Created = x.Created,
                    Modified = x.ModifiedDate,
                    Name = x.Name,
                    Description = x.Description,
                    IsDeleted = x.IsDeleted,
                    IsAdmin = x.IsAdmin
                })
                .ToListAsync();
        }

        public async Task<RoleVM> GetRoleById(String id)
        {
            var role = await _roleManager.Roles
                .Where(x => x.Id == id)
                .Select(x => new RoleVM
                {
                    Id = x.Id,
                    Created = x.Created,
                    Modified = x.ModifiedDate,
                    Name = x.Name,
                    Description = x.Description,
                    IsDeleted = x.IsDeleted,
                    IsAdmin = x.IsAdmin
                })
                .SingleOrDefaultAsync();

            return role ?? new RoleVM();
        }
    }
}
