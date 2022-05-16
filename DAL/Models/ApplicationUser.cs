using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ApplicationUser : IdentityUser <int>
    {
        public string Description { get; set; }
        public ICollection<IdentityUserRole<int>>Roles { get; set; }
        public ICollection<IdentityUserClaim<int>> Claims { get; set; }
    }
}
