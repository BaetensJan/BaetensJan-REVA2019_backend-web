using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Web
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
            var serviceManager = new ServiceManager(services, Configuration);

            serviceManager.AddMvc();
            serviceManager.AddDatabase();
            serviceManager.AddIdentity();
            serviceManager.AddScopes();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ApplicationDbContext context
            /*IServiceProvider serviceProvider*/)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            context.Database.EnsureCreated();

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

//            CreateRoles(app.ApplicationServices);
        }

//        private async void CreateRoles(IServiceProvider serviceProvider)
//        {
//            using (var scope = serviceProvider.CreateScope())
//            {
//                var provider = scope.ServiceProvider;
//                var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
//
//                //initializing custom roles 
////            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//                string[] roleNames = {"Admin", "Teacher", "Group", "SuperAdmin", "School"};
//                IdentityResult roleResult;
//
//                foreach (var roleName in roleNames)
//                {
//                    var roleExist = await roleManager.RoleExistsAsync(roleName);
//                    if (!roleExist)
//                    {
//                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
//                    }
//                }
//            }
//        }
    }
}