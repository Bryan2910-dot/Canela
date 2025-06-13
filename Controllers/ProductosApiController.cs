using Canela.Data;
using Canela.Models;
using Canela.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Canela.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductRecommender _recommender;

        public ProductosApiController(ApplicationDbContext context, ProductRecommender recommender)
        {
            _context = context;
            _recommender = recommender;
        }

        // GET: api/ProductosApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.DbSetProducto.ToListAsync();
        }

        // GET: api/ProductosApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.DbSetProducto.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // POST: api/ProductosApi
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            producto.Status = "en oferta"; // Asegurar el status
            _context.DbSetProducto.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
        }
        // AÑADE ESTOS NUEVOS ENDPOINTS PARA RECOMENDACIONES:

        /// <summary>
        /// Obtiene recomendaciones para un producto específico
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="count">Cantidad de recomendaciones (default: 3)</param>
        [HttpGet("{id}/recomendaciones")]
        public async Task<ActionResult> GetRecomendacionesPorProducto(int id, [FromQuery] int count = 3)
        {
            var producto = await _context.DbSetProducto.FindAsync(id);
            if (producto == null) return NotFound();

            var recomendaciones = _recommender.RecommendProducts(id, count);
            
            return Ok(new
            {
                ProductoBase = new { producto.Id, producto.Name },
                Recomendaciones = recomendaciones.Select(p => new { p.Id, p.Name, p.PriceText, p.Status })
            });
        }

        /// <summary>
        /// Obtiene productos frecuentemente comprados juntos
        /// </summary>
        [HttpGet("frecuentes")]
        public async Task<ActionResult> GetCombinacionesFrecuentes()
        {
            // Primero obtén los datos necesarios de la base de datos
            var preOrdenes = await _context.DbSetPreOrden
                .Include(p => p.Producto)
                .Where(p => p.Producto != null)
                .Select(p => new { p.UserId, Producto = p.Producto })
                .ToListAsync();

            // Luego realiza las operaciones complejas en memoria
            var combinaciones = preOrdenes
                .GroupBy(p => p.UserId)
                .SelectMany(g => g.SelectMany(p1 => g
                    .Where(p2 => p1.Producto.Id != p2.Producto.Id)
                    .Select(p2 => new
                    {
                        Producto1 = p1.Producto.Id,
                        Producto2 = p2.Producto.Id,
                        Nombre1 = p1.Producto.Name,
                        Nombre2 = p2.Producto.Name
                    })))
                .GroupBy(x => new { x.Producto1, x.Producto2 })
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => new
                {
                    g.Key.Producto1,
                    g.Key.Producto2,
                    g.First().Nombre1,
                    g.First().Nombre2,
                    Frecuencia = g.Count()
                })
                .ToList();

            return Ok(combinaciones);
        }
    }
}