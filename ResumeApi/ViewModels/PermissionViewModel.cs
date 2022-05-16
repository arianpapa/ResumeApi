using DAL.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResumeApi.ViewModels
{
    public class PermissionViewModel
    {
        public string Value { get; set; }
        public string Name { get; set; }

		public static explicit operator PermissionViewModel(ApplicationPermission permission)
		{
			return new PermissionViewModel
			{
				Name = permission.Name,
				Value = permission.Value
			};

		}	
	}
}
