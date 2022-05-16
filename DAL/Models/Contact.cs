using DAL.Models;
using DAL.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Vaule { get; set; }
        ContactTypeEnum contactType { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }

    }
}
