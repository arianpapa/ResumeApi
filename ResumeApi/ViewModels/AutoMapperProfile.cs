using AutoMapper;
using DAL.AccountManagement;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResumeApi.ViewModels
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<ApplicationUser, UserViewModel>()
				.ForMember(d => d.Roles, map => map.Ignore());
			CreateMap<UserViewModel, ApplicationUser>()
				.ForMember(d => d.Roles, map => map.Ignore())
				.ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

			CreateMap<ForeignLanguage, ForeignLanguagePostViewModel>()
				.ReverseMap();
			CreateMap<ForeignLanguage, ForeignLanguagePutViewModel>()
				.ReverseMap();
			CreateMap<ForeignLanguage, ForeignLanguageBaseViewModel>()
				.ReverseMap();
			CreateMap<ForeignLanguage, ForeignLanguageEditViewModel>()
				.ReverseMap();

			CreateMap<Skill, SkillPostViewModel>()
				.ReverseMap();
			CreateMap<Skill, SkillPutViewModel>()
				.ReverseMap();
			CreateMap<Skill, SkillBaseViewModel>()
				.ReverseMap();
			CreateMap<Skill, SkillEditViewModel>()
				.ReverseMap();

			CreateMap<Person, PersonPostViewModel>()
				.ReverseMap();
			CreateMap<Person, PersonPutViewModel>()
				.ReverseMap();
			CreateMap<Person, PersonGetViewModel>()
				.ReverseMap();


			CreateMap<ApplicationUser, UserEditViewModel>()
				.ForMember(d => d.Roles, map => map.Ignore());
			CreateMap<UserEditViewModel, ApplicationUser>()
				.ForMember(d => d.Roles, map => map.Ignore())
				.ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

			CreateMap<ApplicationUser, UserPatchViewModel>()
				.ReverseMap();

			CreateMap<ApplicationRole, RoleViewModel>()
				.ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
				.ForMember(d => d.UsersCount, map => map.MapFrom(s => s.Users != null ? s.Users.Count : 0))
				.ReverseMap();
			CreateMap<RoleViewModel, ApplicationRole>()
				.ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

			CreateMap<IdentityRoleClaim<string>, ClaimViewModel>()
				.ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
				.ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
				.ReverseMap();

			CreateMap<ApplicationPermission, PermissionViewModel>()
				.ReverseMap();

			CreateMap<IdentityRoleClaim<string>, PermissionViewModel>()
                .ConvertUsing(s => (PermissionViewModel)ApplicationPermissionCollection.GetSinglePermission(s.ClaimValue));

		}
	}
}
