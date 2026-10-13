using Microsoft.EntityFrameworkCore;
using SmartBusinessProo.Models;
using System.Configuration;

namespace SmartBusinessProo.Data;

public class SmartBusinessContext : DbContext
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        string? cadena = ConfigurationManager
            .ConnectionStrings["SmartBusinessPro"]?.ConnectionString;

        if (string.IsNullOrWhiteSpace(cadena))
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SmartBusinessPro' en App.config.");

        optionsBuilder.UseSqlServer(cadena);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("Productos");
            e.HasKey(p => p.ProductoId);
            e.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
            e.Property(p => p.Precio).HasPrecision(10, 2);
            e.HasOne(p => p.Categoria)
             .WithMany(c => c.Productos)
             .HasForeignKey(p => p.CategoriaId);
        });

        modelBuilder.Entity<Categoria>(e =>
        {
            e.ToTable("Categorias");
            e.HasKey(c => c.CategoriaId);
            e.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        });
    }
}