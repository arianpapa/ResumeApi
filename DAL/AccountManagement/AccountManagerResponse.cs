using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.AccountManagement
{
    public class AccountManagerResponse
    {
        public bool Success { get; set; }
        public IList<string> Errors { get; set; }
    }
}
