using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ResumeApi
{
    public class AuthorizationCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttribute = context.MethodInfo.DeclaringType
                .GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>();

            var hasAuthorize = authorizeAttribute.Any();

            var hasPolicy = authorizeAttribute.Where(p => p.Policy != null).Any();
            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "I pa autorizuar"
                });

                if (hasPolicy)
                {
                    operation.Responses.Add("402", new OpenApiResponse
                    {
                        Description = "I ndaluar"
                    });
                }
                var schema = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement()
                    {
                        [schema] = new[] {"resumeapi"}
                    }
                };
            }
        }
    }
}
