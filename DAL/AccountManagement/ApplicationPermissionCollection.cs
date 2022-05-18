using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.AccountManagement
{
	public class ApplicationPermissionCollection
	{
		//Person Permissions
		public static string CreatePersonPermission = "person.create";
		public static string UpdatePersonPermission = "person.update";
		public static string DeletePersonPermission = "person.delete";
		public static string GetAllPersonPermission = "person.getall";
		public static string GetSinglePersonInformation = "person.getone";

		//Skills Permissions
		public static string CreateSkillPermission = "skill.create";
		public static string UpdateSkillPermission = "skill.update";
		public static string DeleteSkillPermission = "skill.delete";
		public static string GetAllSkillPermission = "skill.getall";
		public static string GetSingleSkillInformation = "skill.getone";

		//WorkingExperience Permissions
		public static string CreateWorkingExperiencePermission = "workingExperience.create";
		public static string UpdateWorkingExperiencePermission = "workingExperience.update";
		public static string DeleteWorkingExperiencePermission = "workingExperience.delete";
		public static string GetAllWorkingExperiencePermission = "workingExperience.getall";
		public static string GetSingleWorkingExperienceInformation = "workingExperience.getone";

		//ForeignLanguage Permissions
		public static string CreateForeignLanguagePermission = "foreignLanguage.create";
		public static string UpdateForeignLanguagePermission = "foreignLanguage.update";
		public static string DeleteForeignLanguagePermission = "foreignLanguage.delete";
		public static string GetAllForeignLanguagePermission = "foreignLanguage.getall";
		public static string GetSingleForeignLanguageInformation = "foreignLanguage.getone";



		public static string ManageUsersPermission = "users.manage";
		public static string ViewAllUsersPermission = "users.view";
		public static string AssignAllowedRolesPermission = "roles.assign";
		public static string ManageAllUsersPermission = "users.manageall";
		public static string ViewAllRolesPermission = "roles.view";
		public static string ManageAllRolesPermission = "roles.manage";
		public static string ViewRoleByRoleNamePermission = "rolebyname.view";

		public static IList<ApplicationPermission> GetListOfPermissions()
		{
			return new List<ApplicationPermission>()
			{
				new ApplicationPermission(CreatePersonPermission, "Krijo person"),
				new ApplicationPermission(UpdatePersonPermission, "Ndrysho person"),
				new ApplicationPermission(DeletePersonPermission, "Fshij person"),
				new ApplicationPermission(GetAllPersonPermission, "Merr listen e personave"),
				new ApplicationPermission(GetSinglePersonInformation, "Merr informacionet per nje person"),


				new ApplicationPermission(CreateSkillPermission, "Krijo skill"),
				new ApplicationPermission(UpdateSkillPermission, "Ndrysho skill"),
				new ApplicationPermission(DeleteSkillPermission, "Fshij skill"),
				new ApplicationPermission(GetAllSkillPermission, "Merr listen e skillsave"),
				new ApplicationPermission(GetSingleSkillInformation, "Merr informacionet per nje skill"),


				new ApplicationPermission(CreateWorkingExperiencePermission, "Krijo workingExperience"),
				new ApplicationPermission(UpdateWorkingExperiencePermission, "Ndrysho workingExperience"),
				new ApplicationPermission(DeleteWorkingExperiencePermission, "Fshij workingExperience"),
				new ApplicationPermission(GetAllWorkingExperiencePermission, "Merr listen e workingExperience"),
				new ApplicationPermission(GetSingleWorkingExperienceInformation, "Merr informacionet per nje workingExperience"),


				new ApplicationPermission(CreateForeignLanguagePermission, "Krijo foreignLanguage"),
				new ApplicationPermission(UpdateForeignLanguagePermission, "Ndrysho foreignLanguage"),
				new ApplicationPermission(DeleteForeignLanguagePermission, "Fshij foreignLanguage"),
				new ApplicationPermission(GetAllForeignLanguagePermission, "Merr listen e foreignLanguages"),
				new ApplicationPermission(GetSingleForeignLanguageInformation, "Merr informacionet per nje foreignLanguage"),



				new ApplicationPermission(ViewAllUsersPermission, "Shiko listen e perdoruesve"),
				new ApplicationPermission(ManageUsersPermission, "Menaxho perdoruesit"),
				new ApplicationPermission(ViewAllUsersPermission, "Listo perdoruesit"),
				new ApplicationPermission(AssignAllowedRolesPermission, "Vendos role per perdoruesit"),
				new ApplicationPermission(ManageAllUsersPermission, "Menaxho te gjithe perdoruesit"),
				new ApplicationPermission(ViewRoleByRoleNamePermission, "Shiko rolin"),
				new ApplicationPermission(ViewAllRolesPermission, "Listo rolet"),
				new ApplicationPermission(ManageAllRolesPermission, "Menaxo rolet")
			};
		}

		public static ApplicationPermission GetSinglePermission(string claim)
		{
			var listOfPermissions = GetListOfPermissions();
			return listOfPermissions.Where(i => i.Value == claim).FirstOrDefault();
		}
		public static bool IsPermissionValid(string permission)
		{
			return GetListOfPermissions().Where(i => i.Value == permission).Any();
		}

	}

	public class ApplicationPermission
	{
		public ApplicationPermission(string value, string name)
		{
			this.Value = value;
			this.Name = name;
		}
		public string Value { get; set; }
		public string Name { get; set; }
	}
}

