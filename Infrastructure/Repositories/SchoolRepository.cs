using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Interfaces;
using SchoolManagementAPI.Models.School;
using SchoolManagementAPI.ViewModels.Application;
using SchoolManagementAPI.ViewModels.Students;
using SchoolManagementAPI.ViewModels.Schools;
using SchoolManagementAPI.ViewModels.User;

namespace SchoolManagementAPI.Infrastructure.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public SchoolRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<SchoolVM> AddSchool(SchoolAddVM model)
        {
            if (await _db.Schools.AnyAsync(x => x.Name == model.Name))
            {
                throw new Exception("Bad Request, There is already a student with the same Id number of mobile number.  Please make sure those fields are unique.");
            }

            var newSchool = _mapper.Map<SchoolAddVM, School>(model);
            await _db.AddAsync(newSchool);
            await _db.SaveChangesAsync();

            return _mapper.Map<School, SchoolVM>(newSchool);
        }

        public async Task<int> DeleteSchool(int id)
        {
            var existingSchool = await _db.Schools.AsTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (existingSchool == null)
            {
                throw new Exception("School does not exist");
            }

            _db.Remove(existingSchool);
            await _db.SaveChangesAsync();

            return id;
        }

        public async Task<SchoolVM> EditSchool(SchoolEditVM model)
        {
            var existingSchool = await _db.Schools.AsTracking().FirstOrDefaultAsync(x => x.Id == model.Id);

            if (existingSchool == null)
            {
                throw new Exception("Student does not exist");
            }

            existingSchool.ModifiedDate = DateTime.UtcNow;
            existingSchool.Name = model.Name;
            existingSchool.Region = model.Region;
            existingSchool.Address = model.Address;
            existingSchool.LandLine1 = model.LandLine1;
            existingSchool.LandLine2 = model.LandLine2;

            await _db.SaveChangesAsync();

            return _mapper.Map<School, SchoolVM>(existingSchool);
        }

        public async Task<List<SchoolVM>> GetAllSchools()
        {
            return await _db.Schools
                .Select(x => new SchoolVM
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    Name = x.Name,
                    Region = x.Region,
                    Address = x.Address,
                    LandLine1 = x.LandLine1,
                    LandLine2 = x.LandLine2,
                    NumberOfStudents = _db.Students.Count(x => x.SchoolId != null && x.SchoolId > 0),
                    NumberOfTeachers = 0
                })
                .ToListAsync();
        }

        public async Task<SchoolVM> GetSchoolById(int id)
        {
            var school = await _db.Schools
                .Where(x => x.Id == id)
                .Select(x => new SchoolVM
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    Name = x.Name,
                    Region = x.Region,
                    Address = x.Address,
                    LandLine1 = x.LandLine1,
                    LandLine2 = x.LandLine2,
                    NumberOfStudents = _db.Students.Count(x => x.SchoolId != null && x.SchoolId > 0),
                    NumberOfTeachers = 0
                })
                .SingleOrDefaultAsync();

            return school ?? new SchoolVM();
        }
    }
}
