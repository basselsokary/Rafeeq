using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.ApplicationContext;

internal class ApplicationDbContextFactory 
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(
            "Data Source=DESKTOP-TQDL9EA\\SQLEXPRESS;Initial Catalog=Rafeeq;Integrated Security=true;Encrypt=False;Trust Server Certificate=True;Trusted_Connection=True;Connect Timeout=60");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}