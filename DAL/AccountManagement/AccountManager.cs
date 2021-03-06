using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.AccountManagement
{
    public class AccountManager : IAccountManager
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public AccountManager(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IHttpContextAccessor httpAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

        }



        public async Task<ApplicationUser> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }


        public async Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
                return null;

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToArrayAsync();

            return (user, roles);
        }


        public async Task<List<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize)
        {
            IQueryable<ApplicationUser> usersQuery = _context.Users
                .Include(u => u.Roles)
                .OrderBy(u => u.UserName);

            if (page != -1)
                usersQuery = usersQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                usersQuery = usersQuery.Take(pageSize);

            var users = await usersQuery.ToListAsync();

            var userRoleIds = users.SelectMany(u => u.Roles.Select(r => r.RoleId)).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();

            return users
                .Select(u => (u, roles.Where(r => u.Roles.Select(ur => ur.RoleId).Contains(r.Id)).Select(r => r.Name).ToArray()))
                .ToList();
        }


        public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user,
            IEnumerable<string> roles, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            user = await _userManager.FindByNameAsync(user.UserName);

            try
            {
                result = await this._userManager.AddToRolesAsync(user, roles.Distinct());
            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user)
        {
            return await UpdateUserAsync(user, null);
        }


        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            if (roles == null) return (true, new string[] { });
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var enumerable = roles as string[] ?? roles.ToArray();
                var rolesToRemove = userRoles.Except(enumerable).ToArray();
                var rolesToAdd = enumerable.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Any())
                {
                    result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (!rolesToAdd.Any()) return (true, new string[] { });
                {
                    result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return (true, new string[] { });
        }

        private string UpdateLastUsedPasswords(string lastUsedPassword, string newPassword)
        {
            var usedPasswordArray = new string[3] { "", "", "" };
            if (lastUsedPassword == null)
            {
                usedPasswordArray[2] = newPassword;
            }
            else
            {
                var usedPasswordsArrayFromDb = lastUsedPassword.Split("->");
                if (usedPasswordsArrayFromDb.Length > 2)
                {
                    usedPasswordArray[0] = usedPasswordsArrayFromDb[1];
                    usedPasswordArray[1] = usedPasswordsArrayFromDb[2];
                    usedPasswordArray[2] = newPassword;
                }
            }
            var lastUsedPasswordUpdated = string.Join("->", Array.ConvertAll<object, string>(usedPasswordArray, Convert.ToString));
            return lastUsedPasswordUpdated;
        }

        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            return !result.Succeeded ? (false, result.Errors.Select(e => e.Description).ToArray()) : (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, new string[] { });
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (await _userManager.CheckPasswordAsync(user, password)) return true;
            if (!_userManager.SupportsUserLockout)
                await _userManager.AccessFailedAsync(user);

            return false;

        }




        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
                return await DeleteUserAsync(user);

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }






        public async Task<ApplicationRole> GetRoleByIdAsync(int roleId)
        {
            return await _roleManager.FindByIdAsync(roleId.ToString());
        }


        public async Task<ApplicationRole> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }


        public async Task<ApplicationRole> GetRoleLoadRelatedAsync(string roleName)
        {
            var role = await _context.Roles
                .Include(r => r.Claims)
                .Include(r => r.Users)
                .Where(r => r.Name == roleName)
                .SingleOrDefaultAsync();
           
            return role;
        }

        public async Task<List<string>> GetRelatedPermission(ApplicationRole role)
        {
            var claims =await _roleManager.GetClaimsAsync(role);
            var listClaims = new List<string>();
            if(claims!=null && claims.Count!=0)
            foreach (var item in claims)
            {
                listClaims.Add(item.Value);
            }
            return listClaims;
        }


        public async Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<ApplicationRole> rolesQuery = _context.Roles
                .Include(r => r.Claims)
                .Include(r => r.Users)
                .OrderBy(r => r.Name);

            if (page != -1)
                rolesQuery = rolesQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                rolesQuery = rolesQuery.Take(pageSize);

            var roles = await rolesQuery.ToListAsync();

            return roles;
        }


        public async Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims)
        {
            claims ??= new string[] { };

            var enumerable = claims as string[] ?? claims.ToArray();
            var invalidClaims = enumerable.Where(i => !ApplicationPermissionCollection.IsPermissionValid(i));
            if (invalidClaims.Any())
                return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });


            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            role = await _roleManager.FindByNameAsync(role.Name);

            foreach (string claim in enumerable.Distinct())
            {
                result = await this._roleManager.AddClaimAsync(role, new Claim("permission", claim));

                if (!result.Succeeded)
                {
                    await DeleteRoleAsync(role);
                    return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(ApplicationRole role, IEnumerable<string> claims)
        {
            if (claims != null)
            {
                string[] invalidClaims = claims.Where(i => !ApplicationPermissionCollection.IsPermissionValid(i)).ToArray();
                if (invalidClaims.Any())
                    return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });
            }


            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            if (claims != null)
            {
                var roleClaims = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == "permission");
                var roleClaimValues = roleClaims.Select(c => c.Value).ToArray();

                var claimsToRemove = roleClaimValues.Except(claims).ToArray();
                var claimsToAdd = claims.Except(roleClaimValues).Distinct().ToArray();

                if (claimsToRemove.Any())
                {
                    foreach (string claim in claimsToRemove)
                    {
                        result = await _roleManager.RemoveClaimAsync(role, roleClaims.Where(c => c.Value == claim).FirstOrDefault());
                        if (!result.Succeeded)
                            return (false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }

                if (claimsToAdd.Any())
                {
                    foreach (string claim in claimsToAdd)
                    {
                        result = await _roleManager.AddClaimAsync(role, new Claim("permission",claim));
                        if (!result.Succeeded)
                            return (false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }
            }

            return (true, new string[] { });
        }


        public async Task<bool> TestCanDeleteRoleAsync(int roleId)
        {
            return !await _context.UserRoles.Where(r => r.RoleId == roleId).AnyAsync();
        }


        public async Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
                return await DeleteRoleAsync(role);

            return (true, new string[] { });
        }

        public bool ValidatePasswordHistory(ApplicationUser user, string hashedPassword, string newPassword)
        {

            PasswordVerificationResult passwordMatch = _userManager.PasswordHasher.VerifyHashedPassword(user, hashedPassword, newPassword);
            if (passwordMatch == PasswordVerificationResult.Success)
            {
                return true;
            }
            else return false;
        }
        public async Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public IList<string> GetUserNames()
        {
            return _context.Users.Select(u => u.UserName).ToList();
        }
    }
}
