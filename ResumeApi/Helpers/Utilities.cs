using IdentityModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ResumeApi.Helpers
{
	public static class Utilities
	{
		private static ILoggerFactory _loggerFactory;

		public static void ConfigureLogger(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}

		public static ILogger CreateLogger<T>()
		{
			//Usage: Utilities.CreateLogger<SomeClass>().LogError(LoggingEvents.SomeEventId, ex, "An error occurred because of xyz");

			if (_loggerFactory == null)
			{
				throw new InvalidOperationException($"{nameof(ILogger)} is not configured. {nameof(ConfigureLogger)} must be called before use");
			}

			return _loggerFactory.CreateLogger<T>();
		}

		public static void QuickLog(string text, string filename)
		{
			var dirPath = Path.GetDirectoryName(filename);

			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			using var writer = File.AppendText(filename);
			writer.WriteLine($"{DateTime.Now} - {text}");
		}

		public static int GetUserId(ClaimsPrincipal user)
		{
			var currentUserId= int.Parse(user.FindFirst(JwtClaimTypes.Subject)?.Value?.Trim());
			return currentUserId;
		}

		public static string[] GetRoles(ClaimsPrincipal identity)
		{
			return identity.Claims
				.Where(c => c.Type == JwtClaimTypes.Role)
				.Select(c => c.Value)
				.ToArray();
		}
	}
}