using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Interfaces;
using SchoolManagementAPI.Models.Student;
using SchoolManagementAPI.ViewModels.Application;
using SchoolManagementAPI.ViewModels.Students;
using SchoolManagementAPI.ViewModels.User;

namespace SchoolManagementAPI.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public StudentRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<StudentVM> AddStudent(StudentAddVM model)
        {
            if (await _db.Students.AnyAsync(x => x.IdentificationNumber == model.IdentificationNumber || x.MobileNumber == model.MobileNumber))
            {
                throw new Exception("Bad Request, There is already a student with the same Id number of mobile number.  Please make sure those fields are unique.");
            }

            var newStudent = _mapper.Map<StudentAddVM, Student>(model);
            await _db.AddAsync(newStudent);
            await _db.SaveChangesAsync();

            return _mapper.Map<Student, StudentVM>(newStudent);
        }

        public async Task<int> DeleteStudent(int id)
        {
            var existingStudent = await _db.Students.AsTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (existingStudent == null)
            {
                throw new Exception("Student does not exist");
            }

            _db.Remove(existingStudent);
            await _db.SaveChangesAsync();

            return id;
        }

        public async Task<StudentVM> EditStudent(StudentEditVM model)
        {
            var existingStudent = await _db.Students.AsTracking().FirstOrDefaultAsync(x => x.Id == model.Id);

            if (existingStudent == null)
            {
                throw new Exception("Student does not exist");
            }

            if (!(await _db.Schools.AnyAsync(x => x.Id == model.SchoolId)))
            {
                throw new Exception("School does not exist");
            }

            existingStudent.ModifiedDate = DateTime.UtcNow;
            existingStudent.FirstName = model.FirstName;
            existingStudent.LastName = model.LastName;
            existingStudent.Email = model.Email;
            existingStudent.IdentificationNumber = model.IdentificationNumber;
            existingStudent.MobileNumber = model.MobileNumber;
            existingStudent.SchoolId = model.SchoolId;

            await _db.SaveChangesAsync();

            return _mapper.Map<Student, StudentVM>(existingStudent);
        }

        public async Task<List<StudentVM>> GetAllStudents()
        {
            ApiResponseModel response = new ApiResponseModel();

            var students = await _db.Students
            .Include(x => x.School)
            .Select(x => new StudentVM
            {
                Id = x.Id,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                FirstName = x.FirstName,
                LastName = x.LastName,
                IdentificationNumber = x.IdentificationNumber,
                Email = x.Email,
                DateOfBirth = x.DateOfBirth,
                MobileNumber = x.MobileNumber,
                SchoolName = x.School != null ? x.School.Name : string.Empty
            })
            .ToListAsync();

            return students;
        }

        public async Task<StudentVM> GetStudentById(int id)
        {
            var student = await _db.Students
                .Where(x => x.Id == id)
                .Select(x => new StudentVM
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    IdentificationNumber = x.IdentificationNumber,
                    Email = x.Email,
                    DateOfBirth = x.DateOfBirth,
                    MobileNumber = x.MobileNumber,
                    SchoolName = x.School != null ? x.School.Name : string.Empty
                })
                .SingleOrDefaultAsync();

            return student ?? new StudentVM();
        }
    }
}
