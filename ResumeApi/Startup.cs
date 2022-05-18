using DAL;
using DAL.AccountManagement;
using DAL.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ResumeApi.Helpers.Account;
using ResumeApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResumeApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>{
                options.UseSqlServer(Configuration["ConnectionStrings:MainConnectionString"], b => b.MigrationsAssembly("ResumeApi"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryPersistedGrants()
                    .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                    .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
                    .AddInMemoryClients(IdentityServerConfig.GetApiClients())
                    .AddAspNetIdentity<ApplicationUser>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiName = "resumeapi";
                    options.SupportedTokens = SupportedTokens.Jwt;
                    options.Authority = Configuration["ApiUrl"];
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("create_person",
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.CreatePersonPermission));

                options.AddPolicy("update_person",
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.UpdatePersonPermission));

                options.AddPolicy("delete_person",
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.DeletePersonPermission));

                options.AddPolicy("get_list_of_persons",
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.GetAllPersonPermission));

                options.AddPolicy("get_one_person",
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.GetSinglePersonInformation));



                options.AddPolicy(Policies.ViewAllSkillsPolicy,
                   policy => policy.RequireClaim("permission", ApplicationPermissionCollection.GetSingleSkillInformation));
                options.AddPolicy(Policies.ViewAllSkillsPolicy,
                   policy => policy.RequireClaim("permission", ApplicationPermissionCollection.GetAllSkillPermission));
                options.AddPolicy(Policies.ManageAllSkillsPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.CreateSkillPermission));
                options.AddPolicy(Policies.ManageAllSkillsPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.UpdateSkillPermission));
                options.AddPolicy(Policies.ManageAllSkillsPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.DeleteSkillPermission));

                options.AddPolicy(Policies.ViewAllWorkingExperiencesPolicy,
                   policy => policy.RequireClaim("permission", ApplicationPermissionCollection.GetSingleWorkingExperienceInformation));
                options.AddPolicy(Policies.ViewAllWorkingExperiencesPolicy,
                   policy => policy.RequireClaim("permission", ApplicationPermissionCollection.GetAllWorkingExperiencePermission));
                options.AddPolicy(Policies.ManageAllWorkingExperiencesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.CreateWorkingExperiencePermission));
                options.AddPolicy(Policies.ManageAllWorkingExperiencesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.UpdateWorkingExperiencePermission));
                options.AddPolicy(Policies.ManageAllWorkingExperiencesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.DeleteWorkingExperiencePermission));

                options.AddPolicy(Policies.ViewAllForeignLanguagesPolicy,
                   policy => policy.RequireClaim("permission", ApplicationPermissionCollection.GetSingleForeignLanguageInformation));
                options.AddPolicy(Policies.ViewAllForeignLanguagesPolicy,
                   policy => policy.RequireClaim("permission", ApplicationPermissionCollection.GetAllForeignLanguagePermission));
                options.AddPolicy(Policies.ManageAllForeignLanguagesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.CreateForeignLanguagePermission));
                options.AddPolicy(Policies.ManageAllForeignLanguagesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.UpdateForeignLanguagePermission));
                options.AddPolicy(Policies.ManageAllForeignLanguagesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.DeleteForeignLanguagePermission));



                options.AddPolicy(Policies.ViewAllUsersPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.ViewAllUsersPermission));
                options.AddPolicy(Policies.AssignAllowedRolesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.AssignAllowedRolesPermission));
                options.AddPolicy(Policies.ManageAllUsersPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.ManageAllUsersPermission));
                options.AddPolicy(Policies.ViewAllRolesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.ViewAllRolesPermission));
                options.AddPolicy(Policies.ManageAllRolesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.ManageAllRolesPermission));
                options.AddPolicy(Policies.ViewRoleByRoleNamePolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.ViewRoleByRoleNamePermission));
                options.AddPolicy(Policies.ManageAllRolesPolicy,
                    policy => policy.RequireClaim("permission", ApplicationPermissionCollection.ManageAllRolesPermission));

            });




            services.AddScoped<IAccountManager, AccountManager>();
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            services.AddControllers();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Resumeapi", Version = "v1" });
                c.OperationFilter<AuthorizationCheckOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl=new Uri("/connect/token", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                { "resumeapi", "Api per restorantin" }
                            }
                        }
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resumeapi v1");
                    c.OAuthClientId("resumeapi");
                    c.OAuthClientSecret("testpass");
                    }
                );
                
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
