using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Infrastructure;
using SchoolManagementAPI.Interfaces;
using NLog;
using SchoolManagementAPI.Infrastructure.Repositories;
using SchoolManagementAPI.ViewModels.Application;
using SchoolManagementAPI.ViewModels.Role;
using Microsoft.AspNetCore.Identity;
using SchoolManagementAPI.Models.User;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAdministrationRepository _administrationRepo;
        private static Logger _logger = LogManager.GetCurrentClassLogger();


        public AdministrationController(ApplicationDbContext db,
            IMapper mapper,
            IAdministrationRepository administrationRepo,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _administrationRepo = administrationRepo;
            _userManager = userManager;
        }

        [HttpGet("roles")]
        public async Task<ActionResult> GetRoles()
        {
            _logger.Info("Get Roles");

            try
            {
                var roles = await _administrationRepo.GetAllRoles();
                return Ok(roles);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not retrieve roles."
                });
            }
        }

        [HttpGet("roles/{id}")]
        public async Task<ActionResult> GetRoleById(string id)
        {
            _logger.Info("Get Student By Id");

            try
            {
                var role = await _administrationRepo.GetRoleById(id);
                return Ok(role);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = $"Something went wrong, could not retrieve role with id {id}."
                });
            }
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddRole(RoleVM model)
        {
            _logger.Info("Add new Role");

            try
            {
                var role = await _administrationRepo.AddRole(model);
                return Ok(role);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not add new role."
                });
            }
        }

        [HttpPut("roles")]
        public async Task<ActionResult> EditRole(RoleVM model)
        {
            _logger.Info("Edit Role");

            try
            {
                var role = await _administrationRepo.EditRole(model);
                return Ok(role);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not edit role."
                });
            }
        }

        [HttpDelete("roles/{id}")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            _logger.Info("Delete role");

            try
            {
                var student = await _administrationRepo.DeleteRole(id);
                return Ok(student);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not delete role."
                });
            }
        }

        [HttpDelete("user/{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            _logger.Info("Delete User");

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    ErrorList error = new ErrorList();

                    var existingUser = await _db.Users.AsTracking()
                        .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                        .Where(x => x.UserName == username)
                        .SingleOrDefaultAsync();

                    if (existingUser != null)
                    {
                        _db.UserRoles.RemoveRange(existingUser.UserRoles);
                        await _userManager.DeleteAsync(existingUser);
                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return Ok(username);
                    }
                    else
                    {
                        await transaction.RollbackAsync();

                        return BadRequest(new ErrorList
                        {
                            Title = "Bad Request",
                            Description = $"User - {username} already exists.",
                            StatusCode = 400
                        });
                    }
                }
                catch (System.Exception e)
                {
                    _logger.Error($"Error on registration: {e.Message}");
                    _logger.Error($"Error Stack Trace: {e.StackTrace}");
                    _logger.Error($"Error Inner Exception: {e.InnerException}");

                    await transaction.RollbackAsync();

                    return BadRequest(new ErrorList
                    {
                        Title = "Bad Request",
                        Description = e.Message.ToString(),
                        StatusCode = 400
                    });
                }
            }
        }
    }
}
