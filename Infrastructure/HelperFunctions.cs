

using System.Security.Claims;

namespace SchoolManagementAPI.Infrastructure
{
    public class HelperFunctions
    {
        private readonly ApplicationDbContext _db;
        public HelperFunctions() { }

        public HelperFunctions(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}