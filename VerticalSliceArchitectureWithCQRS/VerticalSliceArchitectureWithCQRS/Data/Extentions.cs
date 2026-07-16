using Microsoft.EntityFrameworkCore;

namespace VerticalSliceArchitectureWithCQRS.Data
{
    public static class Extentions
    {
        public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.MigrateAsync();

            return app;
        }
    }
}
