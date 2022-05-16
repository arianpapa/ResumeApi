using AutoMapper;
using DAL.AccountManagement;
using DAL.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ResumeApi.Helpers;
using ResumeApi.Helpers.Account;
using ResumeApi.Helpers.Errors;
using ResumeApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResumeApi.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private const string GetUserByIdActionName = "GetUserById";
        private const string GetRoleByIdActionName = "GetRoleById";
        public AccountController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            
        }


        [HttpGet("me/getMe")]
        [Authorize(Policy = Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = Utilities.GetUserId(this.User);
            return Ok(await GetUserViewModelHelper(userId));
        }


        [HttpGet("getUserById/{id}", Name = GetUserByIdActionName)]
        [Authorize(Policy =Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(int id)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Read)).Succeeded)
            //    return new ChallengeResult();

            var userVm = await GetUserViewModelHelper(id);
            
            if (userVm != null)
                return Ok(userVm);
            return NotFound(id);
        }


        [HttpGet("getUserByUsername/{userName}")]
        [Authorize(Policy = Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            var appUser = await _accountManager.GetUserByUserNameAsync(userName);

            //if (!(await _authorizationService.AuthorizeAsync(this.User, appUser?.Id ?? 0, AccountManagementOperations.Read)).Succeeded)
            //    return new ChallengeResult();

            if (appUser == null)
                return NotFound(userName);
            var user = await GetUserById(appUser.Id);
            
            return user;
        }

        [HttpGet("getAllUsers")]
        [Authorize(Policy = Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetUsers()
        {
            var listOfUsers = await GetUsers(-1, -1);
           
            return listOfUsers;
        }

        [HttpGet("getAllUserNames")]
        [Authorize(Policy = Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<string>))]
        public IActionResult GetAllUserNames()
        {
            var listOfUsernames = _accountManager.GetUserNames();
           
            return Ok(listOfUsernames);
        }

        [HttpGet("getAllUsersPaginated/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetUsers(int pageNumber, int pageSize)
        {
            var usersAndRoles = await _accountManager.GetUsersAndRolesAsync(pageNumber, pageSize);

            var usersVm = new List<UserViewModel>();

            foreach (var (user, roles) in usersAndRoles)
            {
                var userVm = _mapper.Map<UserViewModel>(user);
                userVm.Roles = roles;

                usersVm.Add(userVm);
            }
            
            return Ok(usersVm);
        }


        [HttpPut("updateMe")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserEditViewModel user)
        {
            return await UpdateUser(Utilities.GetUserId(User), user);
        }

        [HttpPut("updateUser/{id}")]
        [Authorize(Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserEditViewModel user)
        {
            var appUser = await _accountManager.GetUserByIdAsync(id);
            var currentRoles = appUser != null ? (await _accountManager.GetUserRolesAsync(appUser)).ToArray() : null;

            //var manageUsersPolicy =  _authorizationService.AuthorizeAsync(User, id, AccountManagementOperations.Update);
            //var assignRolePolicy =   _authorizationService.AuthorizeAsync(User, (user.Roles, currentRoles), Policies.AssignAllowedRolesPolicy);


            //if ((await Task.WhenAll(manageUsersPolicy, assignRolePolicy)).Any(r => !r.Succeeded))
            //    return new ChallengeResult();


            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!(user.Id==0) && id != user.Id)
                return BadRequest("Conflicting user id in parameter and model data");

            if (appUser == null)
                return NotFound(id);

            var isPasswordChanged = !string.IsNullOrWhiteSpace(user.NewPassword);
            var isUserNameChanged = !appUser.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase);

            //if (Utilities.GetUserId(this.User) == id)
            //{
                if (isPasswordChanged)
                {
                    var error = CheckPasswordRegex(user.NewPassword);
                    if (error != null) return BadRequest(error);
                }

                if (!ModelState.IsValid) return BadRequest(ModelState);
                _mapper.Map(user, appUser);

                var result = await _accountManager.UpdateUserAsync(appUser, user.Roles);
                if (result.Succeeded)
                {
                    if (isPasswordChanged)
                    {
                        if (!string.IsNullOrWhiteSpace(user.CurrentPassword))
                            result = await _accountManager.UpdatePasswordAsync(appUser, user.CurrentPassword, user.NewPassword);
                        else
                            result = await _accountManager.ResetPasswordAsync(appUser, user.NewPassword);
                    }

                    if (result.Succeeded)
                    {

                        return NoContent();
                    }
                }

                AddError(result.Errors);

                return BadRequest(ModelState);
           // }
            //return BadRequest("duhet te logoheni me kete user per ta update");
        }


        //[HttpPatch("updateMePatch")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> UpdateCurrentUser([FromBody] JsonPatchDocument<UserPatchViewModel> patch)
        //{
        //    return await UpdateUser(Utilities.GetUserId(User), patch);
        //}


        [HttpPatch("updateUserPatch/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] JsonPatchDocument<UserPatchViewModel> patch)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, id, AccountManagementOperations.Update)).Succeeded)
                return new ChallengeResult();


            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (patch == null)
                return BadRequest($"{nameof(patch)} cannot be null");


            var appUser = await _accountManager.GetUserByIdAsync(int.Parse( id));

            if (appUser == null)
                return NotFound(id);


            var userPvm = _mapper.Map<UserPatchViewModel>(appUser);
            patch.ApplyTo(userPvm, (e) => AddError(e.ErrorMessage));

            if (!ModelState.IsValid) return BadRequest(ModelState);
            _mapper.Map(userPvm, appUser);

            var (succeeded, errors) = await _accountManager.UpdateUserAsync(appUser);
            if (succeeded)
                return NoContent();


            AddError(errors);

            return BadRequest(ModelState);
        }


        [HttpPost("registerUser")]
        [Authorize(Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(201, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Register([FromBody] UserEditViewModel user)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, (user.Roles, new string[] { }), Policies.AssignAllowedRolesPolicy)).Succeeded)
                return new ChallengeResult();


            if (!ModelState.IsValid) return BadRequest(ModelState);

            var error = CheckPasswordRegex(user.NewPassword);
            if (error != null) return BadRequest(error);

            var appUser = _mapper.Map<ApplicationUser>(user);

            var (succeeded, errors) = await _accountManager.CreateUserAsync(appUser, user.Roles, user.NewPassword);
            if (succeeded)
            {
                var userVm = await GetUserViewModelHelper(appUser.Id);
          
                return CreatedAtAction(GetUserByIdActionName, new { id = userVm.Id }, userVm);
            }
           
            AddError(errors);

            return BadRequest(ModelState);
        }


        [HttpDelete("deleteUser/{id}")]
        [Authorize(Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Delete)).Succeeded)
            //    return new ChallengeResult();


            var appUser = await _accountManager.GetUserByIdAsync(int.Parse( id));

            if (appUser == null)
                return NotFound(id);

            var userVm = await GetUserViewModelHelper(appUser.Id);
       
            var (succeeded, errors) = await _accountManager.DeleteUserAsync(appUser);
            if (!succeeded)
                throw new Exception("The following errors occurred whilst deleting user: " + string.Join(", ", errors));


            return Ok(userVm);
        }


        [HttpPut("unblockUser/{id}")]
        [Authorize(Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UnblockUser(string id)
        {
            var appUser = await _accountManager.GetUserByIdAsync(int.Parse( id));

            if (appUser == null)
                return NotFound(id);

            appUser.LockoutEnd = null;
            var (succeeded, errors) = await _accountManager.UpdateUserAsync(appUser);
            if (!succeeded)
                throw new Exception("The following errors occurred whilst unblocking user: " + string.Join(", ", errors));


            return NoContent();
        }


        


     





        [HttpGet("getRoleById/{id}", Name = GetRoleByIdActionName)]
        [Authorize(Policies.AssignAllowedRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var appRole = await _accountManager.GetRoleByIdAsync(int.Parse( id));

            //if (!(await _authorizationService.AuthorizeAsync(User, appRole?.Name ?? "", Policies.ViewRoleByRoleNamePolicy)).Succeeded)
            //    return new ChallengeResult();

            if (appRole == null)
                return NotFound(id);

            return await GetRoleByName(appRole.Name);
        }


        [HttpGet("getRoleByName/{name}")]
        [Authorize(Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            //if (!(await _authorizationService.AuthorizeAsync(User, name, Policies.ViewRoleByRoleNamePolicy)).Succeeded)
            //    return new ChallengeResult();


            var roleVm = await GetRoleViewModelHelper(name);

            if (roleVm == null)
                return NotFound(name);

            
            return Ok(roleVm);
        }


        [HttpGet("getAllRoles")]
        [Authorize(Policies.AssignAllowedRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles()
        {
            return await GetRoles(-1, -1);
        }


        [HttpGet("getAllRolesPaginated/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles(int pageNumber, int pageSize)
        {
            var roles = await _accountManager.GetRolesLoadRelatedAsync(pageNumber, pageSize);
            var listOfRoles = _mapper.Map<List<RoleViewModel>>(roles);
           
            return Ok(listOfRoles);
        }


        [HttpPut("updateRole/{id}")]
        [Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleViewModel role)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (role == null)
                return BadRequest($"{nameof(role)} cannot be null");

            if (!string.IsNullOrWhiteSpace(role.Id) && id != role.Id)
                return BadRequest("Conflicting role id in parameter and model data");



            var appRole = await _accountManager.GetRoleByIdAsync(int.Parse( id));

            if (appRole == null)
                return NotFound(id);


            _mapper.Map(role, appRole);

            var (succeeded, errors) = await _accountManager.UpdateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
            if (succeeded)
            {
                
                return NoContent();
            }

            AddError(errors);
         
            return BadRequest(ModelState);
        }


        [HttpPost("createRole")]
        [Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(201, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRole([FromBody] RoleViewModel role)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (role == null)
                return BadRequest($"{nameof(role)} cannot be null");


            var appRole = _mapper.Map<ApplicationRole>(role);

            var (succeeded, errors) =
                await _accountManager.CreateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
            if (succeeded)
            {
                var roleVm = await GetRoleViewModelHelper(appRole.Name);
       
                return CreatedAtAction(GetRoleByIdActionName, new { id = roleVm.Id }, roleVm);
            }

            AddError(errors);
            
            return BadRequest(ModelState);
        }


        [HttpDelete("deleteRole/{id}")]
        [Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var appRole = await _accountManager.GetRoleByIdAsync(int.Parse( id));

            if (appRole == null)
                return NotFound(id);

            if (!await _accountManager.TestCanDeleteRoleAsync(int.Parse(id)))
                return BadRequest("Role cannot be deleted. Remove all users from this role and try again");


            var roleVm = await GetRoleViewModelHelper(appRole.Name);

            var (succeeded, errors) = await _accountManager.DeleteRoleAsync(appRole);
            if (!succeeded)
            {
               
                throw new Exception("The following errors occurred whilst deleting role: " + string.Join(", ", errors));
            }

            
            return Ok(roleVm);
        }


        [HttpGet("get-all-permissions")]
        [Authorize(Policies.AssignAllowedRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PermissionViewModel>))]
        public IActionResult GetAllPermissions()
        {
            return Ok(_mapper.Map<List<PermissionViewModel>>(ApplicationPermissionCollection.GetListOfPermissions()));
        }

        private async Task<UserViewModel> GetUserViewModelHelper(int userId)
        {
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
                return null;

            var userVm = _mapper.Map<UserViewModel>(userAndRoles.Value.User);
            userVm.Roles = userAndRoles.Value.Roles;

            return userVm;
        }


        private async Task<RoleViewModel> GetRoleViewModelHelper(string roleName)
        {
            var role = await _accountManager.GetRoleLoadRelatedAsync(roleName);
            var claims = await _accountManager.GetRelatedPermission(role);
            var result = _mapper.Map<RoleViewModel>(role);
            var permissions = new List<ApplicationPermission>();
            if(claims!=null && claims.Count!=0)
            foreach (var item in claims)
            {
                var applicationPermission = new ApplicationPermission(item, item);
                permissions.Add(applicationPermission);
            }
            result.Permissions = permissions.ToArray();
            return role != null ? result: null;
        }


        private void AddError(IEnumerable<string> errors, string key = "")
        {
            foreach (var error in errors)
            {
                AddError(error, key);
            }
        }

        private void AddError(string error, string key = "")
        {
            ModelState.AddModelError(key, error);
        }
        private static ErrorModel CheckPasswordRegex(string password)
        {
            var reg = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$");
            if (!reg.IsMatch(password))
            {
                var error = new ErrorModel()
                {
                    Code = ErrorCodes.PASSWORD_IS_NOT_VALID,
                    Message = ErrorMessages.PASSWORD_IS_NOT_VALID
                };
                return error;
            }
            else
            {
                return null;
            }
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return buffer3.SequenceEqual(buffer4);
        }

        private ErrorModel CheckPasswordHistory(ApplicationUser user, string lastUsedPassword, string newPassword)
        {
            if (lastUsedPassword == null) return null;
            var passSameAsActual = _accountManager.ValidatePasswordHistory(user, user.PasswordHash, newPassword);
            if (passSameAsActual)
                return null;

            return null;
        }

        private static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

    }
}
