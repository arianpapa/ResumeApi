using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.AccountManagement
{
    public interface IAccountManager
    {
		Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
		Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
		Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles,
			string password);
		Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role);
		Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string roleName);
		Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(ApplicationUser user);
		Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(int userId);
		Task<ApplicationRole> GetRoleByIdAsync(int roleId);
		Task<ApplicationRole> GetRoleByNameAsync(string roleName);
		Task<ApplicationRole> GetRoleLoadRelatedAsync(string roleName);
		Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize);
		Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(int userId);
		Task<ApplicationUser> GetUserByEmailAsync(string email);
		Task<ApplicationUser> GetUserByIdAsync(int userId);
		Task<ApplicationUser> GetUserByUserNameAsync(string userName);
		Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
		IList<string> GetUserNames();
		Task<List<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize);
		Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string newPassword);
		Task<bool> TestCanDeleteRoleAsync(int roleId);
		Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
		Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
		Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user);
		Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles);
		public bool ValidatePasswordHistory(ApplicationUser user, string hashedPassword, string newPassword);
		Task<List<string>> GetRelatedPermission(ApplicationRole role);


	}
}
