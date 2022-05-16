using DAL.Models;
using DAL.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public GenderEnum Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nid { get; set; }
        public string PlaceOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Adress { get; set; }
        ICollection<Contact>Contacts { get; set; }
        ICollection<WorkingExperience> WorkingExperiences { get; set; }
        ICollection<Education> Educations { get; set; }
        ICollection<Skill> Skills { get; set; }
        ICollection<ForeignLanguage> ForeignLanguages { get; set; }

    }
}
