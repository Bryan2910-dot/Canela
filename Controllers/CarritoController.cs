using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Canela.Data;
using Canela.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Canela.Controllers
{
    [Authorize]
    public class CarritoController : Controller
    {
        private readonly ILogger<CarritoController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CarritoController(
            ILogger<CarritoController> logger,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = "/Carrito" });
            }

            var items = await _context.DbSetPreOrden
                .Include(p => p.Producto)  // Asegura que carga el Producto relacionado
                .Where(w => w.UserId == userId && w.Status == "PENDIENTE")
                .ToListAsync();

            var total = items.Sum(c => c.Cantidad * c.Precio);

            ViewBag.MontoTotal = total;
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var producto = await _context.DbSetProducto.FindAsync(id);
            if (producto == null) return NotFound();

            var itemExistente = await _context.DbSetPreOrden
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(p => p.Producto != null && 
                                       p.Producto.Id == id && 
                                       p.UserId == userId && 
                                       p.Status == "PENDIENTE");

            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
                itemExistente.PrecioText = producto.Price.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                _context.DbSetPreOrden.Add(new PreOrden
                {
                    Producto = producto,  // Asigna el objeto Producto completo
                    PrecioText = producto.Price.ToString(CultureInfo.InvariantCulture),
                    Cantidad = 1,
                    UserId = userId,
                    Status = "PENDIENTE"
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.DbSetPreOrden.FindAsync(id);
            if (item == null) return NotFound();

            _context.DbSetPreOrden.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int cantidad)
        {
            var item = await _context.DbSetPreOrden.FindAsync(id);
            if (item == null) return NotFound();

            item.Cantidad = cantidad;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}