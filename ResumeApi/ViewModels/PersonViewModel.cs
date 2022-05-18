using DAL.Models.Enum;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResumeApi.ViewModels
{
    public class PersonBaseViewModel
    {
        [Required(ErrorMessage = "Emri është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Emri  duhet të jete midis 1 dhe 500 karakteresh.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Mbiemri është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Mbiemri  duhet të jete midis 1 dhe 500 karakteresh.")]
        public string Surname { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Nid është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Nid  duhet të jete midis 1 dhe 500 karakteresh.")]
        public string Nid { get; set; }
        [Required(ErrorMessage = "Vendi i lindjes është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Vendi i lindjes  duhet të jete midis 1 dhe 500 karakteresh.")]
        public string PlaceOfBirth { get; set; }
        [Required(ErrorMessage = "Qyteti është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Qyteti duhet të jete midis 1 dhe 500 karakteresh.")]
        public string City { get; set; }
        [Required(ErrorMessage = "Shteti është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Shteti duhet të jete midis 1 dhe 500 karakteresh.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Adresa është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Adresa duhet të jete midis 1 dhe 500 karakteresh.")]
        public string Adress { get; set; }
    }
    public class PersonEditViewModel : PersonBaseViewModel
    {
        public int Id { get; set; }
    }
    public class PersonPostViewModel : PersonBaseViewModel
    {

    }
    public class PersonPutViewModel : PersonBaseViewModel
    {

    }




}
