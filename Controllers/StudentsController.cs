using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Infrastructure;
using SchoolManagementAPI.Interfaces;
using SchoolManagementAPI.Models.Student;
using SchoolManagementAPI.ViewModels.Students;
using NLog;
using SchoolManagementAPI.Infrastructure.Repositories;
using SchoolManagementAPI.ViewModels.Application;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseApiController
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();


        public StudentController(ApplicationDbContext db, IMapper mapper, IStudentRepository studentRepository)
        {
            _db = db;
            _mapper = mapper;
            _studentRepository = studentRepository;
        }

        [HttpGet("students")]
        public async Task<ActionResult> GetStudents()
        {
            _logger.Info("Get Students");

            try
            {
                var students = await _studentRepository.GetAllStudents();
                return Ok(students);
            }
            catch (System.Exception e)
            {
                _logger.Error($"Error message: {e.Message}");
                _logger.Error($"Error stack trace: {e.StackTrace}");
                _logger.Error($"Error inner exception: {e.InnerException}");

                return BadRequest(new ErrorList
                {
                    Title = "Bad Request",
                    Description = "Something went wrong, could not retrieve students."
                });
            }
        }

        [HttpGet("students/{id}")]
        public async Task<ActionResult> GetStudentById(int id)
        {
            _logger.Info("Get Student By Id");

            try
            {
                var student = await _studentRepository.GetStudentById(id);
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
                    Description = $"Something went wrong, could not retrieve student with id {id}."
                });
            }
        }

        [HttpPost("students")]
        public async Task<ActionResult> AddStudent(StudentAddVM model)
        {
            _logger.Info("Add new Student");

            try
            {
                var student = await _studentRepository.AddStudent(model);
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
                    Description = "Something went wrong, could not add new student."
                });
            }
        }

        [HttpPut("students")]
        public async Task<ActionResult> EditStudent(StudentEditVM model)
        {
            _logger.Info("Edit Student");

            try
            {
                var student = await _studentRepository.EditStudent(model);
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
                    Description = "Something went wrong, could not edit student."
                });
            }
        }

        [HttpDelete("students/{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            _logger.Info("Delete student");

            try
            {
                var student = await _studentRepository.DeleteStudent(id);
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
                    Description = "Something went wrong, could not delete student."
                });
            }
        }
    }
}
