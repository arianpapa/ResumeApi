using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Education
    {
        public int Id { get; set; }
        public string EduInstitutionName { get; set; }
        public string EduInstitutionNameAddress { get; set; }
        public string DegreeTitle { get; set; }
        public DateTime StandartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
