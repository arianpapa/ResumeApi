using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResumeApi.ViewModels
{
	public class UserViewModel : UserBaseViewModel
	{
		public bool IsLockedOut { get; set; }
		public string[] Roles { get; set; }
	}

	public class UserEditViewModel : UserBaseViewModel
	{
		public string CurrentPassword { get; set; }

		public string NewPassword { get; set; }
		
		public string[] Roles { get; set; }
	}

	public class UserPatchViewModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Institution { get; set; }
		public string PersonalNr { get; set; }
		public string Email { get; set; }
		public string BirthDate { get; set; }
		public string JobTitle { get; set; }
		public string PhoneNumber { get; set; }
		public string OfficePhoneNumber { get; set; }
		public string Configuration { get; set; }
	}

	public abstract class UserBaseViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Emri i institucionit është fushë e detyrueshme."), StringLength(500, MinimumLength = 1, ErrorMessage = "Emërtimi i institucionit  duhet të jete midis 1 dhe 500 karakteresh.")]
		public string Institution { get; set; }

		[Required(ErrorMessage = "Emri i përdoruesit është fushë e detyrueshme."), StringLength(200, MinimumLength = 2, ErrorMessage = "Emri i përdoruesit duhet të jete midis 2 dhe 200 karakteresh.")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Emri i personit është fushë e detyrueshme."), StringLength(200, MinimumLength = 2, ErrorMessage = "Emri i personit duhet të jete midis 2 dhe 200 karakteresh.")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Mbiemri i personit është fushë e detyrueshme."), StringLength(200, MinimumLength = 2, ErrorMessage = "Mbiemri i personit duhet të jete midis 2 dhe 200 karakteresh.")]
		public string Lastname { get; set; }

		[Required(ErrorMessage = "Numri personal është fushë e detyrueshme."), StringLength(200, MinimumLength = 2, ErrorMessage = "numri personal duhet të jete midis 2 dhe 200 karakteresh.")]
		public string PersonalNr { get; set; }

		[Required(ErrorMessage = "Email-i ështe fushë e detyrueshme."), StringLength(200, ErrorMessage = "Email-i nuk mund të ketë më shume se 200 karaktere."), EmailAddress(ErrorMessage = "Email-i nuk ështe i vlefshëm.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Funksioni është fushë e detyrueshme."), StringLength(200, ErrorMessage = "Funksioni nuk mund të ketë më shume se 200 karaktere.")]
		public string JobTitle { get; set; }

		[Required(ErrorMessage = "Datëlindja është fushë e detyrueshme.")]
		public DateTime BirthDate { get; set; }
		public string PhoneNumber { get; set; }
		public string OfficePhoneNumber { get; set; }
		public string Configuration { get; set; }
		public bool IsEnabled { get; set; }
	}

}