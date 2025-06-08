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
}
