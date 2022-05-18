using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResumeApi.ViewModels
{
    public class ForeignLanguageBaseViewModel
    {
        [Required(ErrorMessage = "Gjuha është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Gjuha  duhet të jete midis 1 dhe 500 karakteresh.")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Niveli është fushë e detyrueshme."), StringLength(5, MinimumLength = 2, ErrorMessage = "Niveli duhet të jete midis 2 dhe 5 karakteresh.")]
        public string Level { get; set; }
        public int PersonId { get; set; }
    }

    public class ForeignLanguageEditViewModel : ForeignLanguageBaseViewModel
    {
        public int Id { get; set; }
    }
    public class ForeignLanguagePostViewModel : ForeignLanguageBaseViewModel
    {

    }
    public class ForeignLanguagePutViewModel : ForeignLanguageBaseViewModel
    {

    }
}
