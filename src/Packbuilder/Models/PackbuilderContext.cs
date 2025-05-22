using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Packbuilder.Models;

public class PackbuilderContext : DbContext
{
    public PackbuilderContext(DbContextOptions<PackbuilderContext> options)
        : base(options)
    {
        
    }

    private void UpdateTimestamps()
    {
        IEnumerable<EntityEntry<BaseModel>> entries = ChangeTracker.Entries().OfType<EntityEntry<BaseModel>>();

        foreach (EntityEntry<BaseModel> entry in entries)
        {
            BaseModel entity = entry.Entity;
            entity.Bump();
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Modpack> Modpacks { get; set; }
    public DbSet<Mod> Mods { get; set; }
    public DbSet<Modification> Modifications { get; set; }
    public DbSet<Suggestion> Suggestions { get; set; }
    public DbSet<Version> Versions { get; set; }
    public DbSet<VersionMod> VersionMods { get; set; }
}