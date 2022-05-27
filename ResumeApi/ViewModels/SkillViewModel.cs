using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResumeApi.ViewModels
{
    public class SkillBaseViewModel
    {
        public string Value { get; set; }
        public int PersonId { get; set; }
        public bool IsSoftSkill { get; set; }
        public int Level { get; set; }
    }

    public class SkillEditViewModel : SkillBaseViewModel
    {
        public int Id { get; set; }
    }
    public class SkillPostViewModel : SkillBaseViewModel
    {

    }
    public class SkillPutViewModel : SkillBaseViewModel
    {

    }
}
