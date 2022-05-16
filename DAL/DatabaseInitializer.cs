using DAL.AccountManagement;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DatabaseInitializer : IDatabaseInitializer 
    {
        private IAccountManager _accountManager;
        private AppDbContext _context;

        public DatabaseInitializer(IAccountManager accountManager, AppDbContext dbContext)
        {
            _accountManager = accountManager;
            _context = dbContext;
        }
        public async Task SeedDatabase()
        {
            //_context.Database.MigrateAsync().Wait();
            var role = new ApplicationRole()
            {
                Name = "administrator"
            };
            List<string> claims = new List<string> { };
             _accountManager.CreateRoleAsync(role, claims).Wait();

            var user = new ApplicationUser()
            {
                UserName = "administrator",
                Description = "administratori i sistemit",
                Email = "admin@email.com",
                PhoneNumber = "1234",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            List<string> roles = new List<string> { "administrator" };
             _accountManager.CreateUserAsync(user, roles, "tempP@ss123").Wait();
        }

    }
}
