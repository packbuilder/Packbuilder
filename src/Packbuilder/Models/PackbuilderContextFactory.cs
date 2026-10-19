using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Packbuilder.Models;

public class PackbuilderContextFactory
    : IDesignTimeDbContextFactory<PackbuilderContext>
{
    public PackbuilderContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<PackbuilderContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=packbuilder;Username=postgres;Password=postgres");

        return new PackbuilderContext(optionsBuilder.Options);
    }
}