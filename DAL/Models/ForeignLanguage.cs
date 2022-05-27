﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ForeignLanguage
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Level { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
