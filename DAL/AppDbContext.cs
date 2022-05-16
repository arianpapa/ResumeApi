using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<WorkingExperience> WorkingExperiences { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ForeignLanguage> ForeignLanguages { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        }
    }
}
