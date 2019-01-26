using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Web
{
    public class Program
    {
     /*   public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            using (ApplicationDbContext context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()))
            {
                new dataInitializer(context).InitializeData();
            }
        }
        */
       //zonder de dataInitializer
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        
        }
        
       
}