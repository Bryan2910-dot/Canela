using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Canela.Data;
using Canela.Models;

namespace Canela.Service
{
    public class ProductoService
    {
        private readonly ILogger<ProductoService> _logger;
        private readonly ApplicationDbContext _context;

        public ProductoService(ILogger<ProductoService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<Producto>> GetAll()
        {
            return await _context.DbSetProducto
                .Where(p => p.Status == "en oferta")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Producto?> GetById(int id)
        {
            _logger.LogInformation("Fetching product with ID {0} from the database.", id);
            if (_context.DbSetProducto == null)
                return null;
            var producto = await _context.DbSetProducto.FindAsync(id);
            if (producto == null)
                return null;
            _logger.LogInformation("Fetched product with ID {0} from the database.", id);
            return producto;
        }

        public async Task<bool> Add(Producto producto)
        {
            _logger.LogInformation("Adding a new product to the database.");
            if (_context.DbSetProducto == null)
                return false;
            await _context.DbSetProducto.AddAsync(producto);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added a new product to the database with ID {0}.", producto.Id);
            return true;
        }

        public async Task<bool> Update(Producto producto)
        {
            _logger.LogInformation("Updating product with ID {0} in the database.", producto.Id);
            if (_context.DbSetProducto == null)
                return false;
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated product with ID {0} in the database.", producto.Id);
            return true;
        }
    }
}