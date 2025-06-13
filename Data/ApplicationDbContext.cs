using Canela.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Canela.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DetalleOrden> DbSetDetalleOrden { get; set; }
    public DbSet<Producto> DbSetProducto { get; set; }
    public DbSet<PreOrden> DbSetPreOrden { get; set; }
    public DbSet<Pago> DbSetPago { get; set; }
    public DbSet<Orden> DbSetOrden { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SOLUCIÓN EXCLUSIVA PARA EL PROBLEMA DE BOOLEANOS
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(bool))
                {
                    property.SetProviderClrType(typeof(int)); // Convierte bool a int en PostgreSQL
                }
            }
        }
    }
}