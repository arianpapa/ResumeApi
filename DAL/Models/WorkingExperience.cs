using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class WorkingExperience
    {
        public int Id { get; set; }
        public string ShortDescription { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationAdress { get; set; }
        public DateTime StandartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }

    }
}
