using System.Security.Cryptography.X509Certificates;
using Dengue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dengue.Infrastructure.Configurations;

public class DengueAlertConfiguration : IEntityTypeConfiguration<DengueAlert>{
    public void Configure(EntityTypeBuilder<DengueAlert> builder){
        builder.ToTable("DengueAlerts");

        builder.HasKey(x => x.Id);

        //Chave
        builder.HasIndex(x => new {x.Ew, x.Ey}).IsUnique();

        builder.Property(x => x.Ew).IsRequired();
        builder.Property(x => x.Ey).IsRequired();
        builder.Property(x => x.DataIniSE).IsRequired();
        builder.Property(x => x.CasosEst).IsRequired();
        builder.Property(x => x.Casos).IsRequired();
        builder.Property(x => x.Nivel).IsRequired();
        builder.Property(x => x.ImportedAt).IsRequired();

        builder.Property(x => x.CasosEst).HasPrecision(18, 4);
        builder.Property(x => x.PRt1).HasPrecision(18, 6);
        builder.Property(x => x.PInc100k).HasPrecision(18, 6);
        builder.Property(x => x.Rt).HasPrecision(18, 6);
        builder.Property(x => x.Pop).HasPrecision(18, 4);
    }
}