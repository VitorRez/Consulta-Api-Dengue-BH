using System.Dynamic;
using Dengue.Domain.Entities;
using Dengue.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dengue.Infrastructure.Context;

public class DengueDbContext : DbContext{
    public DengueDbContext(DbContextOptions<DengueDbContext> options) : base(options){}
    public DbSet<DengueAlert> DengueAlerts => Set<DengueAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DengueDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}