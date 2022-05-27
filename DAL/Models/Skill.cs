using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int PersonId { get; set; }
        public bool IsSoftSkill { get; set; }
        public int Level { get; set; }
        public Person Person { get; set; }
    }
}
