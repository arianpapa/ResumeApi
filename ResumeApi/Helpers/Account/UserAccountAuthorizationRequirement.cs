using DAL.AccountManagement;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ResumeApi.Helpers
{
	public class UserAccountAuthorizationRequirement : IAuthorizationRequirement
	{
		public UserAccountAuthorizationRequirement(string operationName)
		{
			this.OperationName = operationName;
		}


		public string OperationName { get; private set; }
	}



	public class ViewUserAuthorizationHandler : AuthorizationHandler<UserAccountAuthorizationRequirement, int>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAccountAuthorizationRequirement requirement, int targetUserId)
		{
			if (context.User == null || requirement.OperationName != AccountManagementOperations.ReadOperationName)
				return Task.CompletedTask;

			if (context.User.HasClaim("permission", ApplicationPermissionCollection.ViewAllUsersPermission) || GetIsSameUser(context.User, targetUserId))
				context.Succeed(requirement);
			return Task.CompletedTask;
		}
		private bool GetIsSameUser(ClaimsPrincipal user, int targetUserId)
		{
			//if (string.IsNullOrWhiteSpace(targetUserId))
			//	return false;

			if (targetUserId == 0)
				return false;

			return Utilities.GetUserId(user) == targetUserId;
		}
	}
}
