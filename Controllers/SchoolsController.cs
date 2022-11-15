using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Infrastructure;
using SchoolManagementAPI.Interfaces;
using SchoolManagementAPI.Models.School;
using SchoolManagementAPI.ViewModels.Schools;
using NLog;
using SchoolManagementAPI.Infrastructure.Repositories;
using SchoolManagementAPI.ViewModels.Application;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ISchoolRepository _schoolRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();


        public SchoolController(ApplicationDbContext db, IMapper mapper, ISchoolRepository schoolRepository)
        {
            _db = db;
            _mapper = mapper;
            _schoolRepository = schoolRepository;
        }

        [HttpGet("schools")]
        public async Task<ActionResult> GetSchools()
        {
            _logger.Info("Get Schools");

            try
            {
                var schools = await _schoolRepository.GetAllSchools();
                return Ok(schools);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not retrieve schools."
                });
            }
        }

        [HttpGet("schools/{id}")]
        public async Task<ActionResult> GetSchoolById(int id)
        {
            _logger.Info("Get School By Id");

            try
            {
                var school = await _schoolRepository.GetSchoolById(id);
                return Ok(school);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = $"Something went wrong, could not retrieve school with id {id}."
                });
            }
        }

        [HttpPost("schools")]
        public async Task<ActionResult> AddSchool(SchoolAddVM model)
        {
            _logger.Info("Add new school");

            try
            {
                var school = await _schoolRepository.AddSchool(model);
                return Ok(school);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not add new school."
                });
            }
        }

        [HttpPut("schools")]
        public async Task<ActionResult> EditSchool(SchoolEditVM model)
        {
            _logger.Info("Edit school");

            try
            {
                var school = await _schoolRepository.EditSchool(model);
                return Ok(school);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not edit school."
                });
            }
        }

        [HttpDelete("schools/{id}")]
        public async Task<ActionResult> DeleteSchool(int id)
        {
            _logger.Info("Delete school");

            try
            {
                var school = await _schoolRepository.DeleteSchool(id);
                return Ok(school);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not delete school."
                });
            }
        }
    }
}
