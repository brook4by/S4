using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
             }
        }

        private static void SeedData(AppDbContext context, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("-->Attempting to apply migrations....");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"--> Could not run migrations: {e.Message}");
                }

            }
            if (!context.Platforms.Any()) 
            {
                Console.WriteLine("Seeding data...");

                context.Platforms.AddRange(
                    new Models.Platform() { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
                    new Models.Platform() { Name = "Sql Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Models.Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                    );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("We already have data");
            }

        }
    }
}
