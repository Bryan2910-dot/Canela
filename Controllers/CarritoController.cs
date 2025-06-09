using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Canela.Data;
using Canela.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Canela.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ILogger<CarritoController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CarritoController(ILogger<CarritoController> logger,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User); // Obtiene el ID del usuario autenticado
            if (userId == null)
            {
                ViewData["Message"] = "Por favor debe loguearse antes de agregar un producto";
                return RedirectToAction("Index", "Catalogo");
            }

            var items = from o in _context.DbSetPreOrden select o;
            items = items.Include(p => p.Producto)
                        .Where(w => w.UserId.Equals(userId) && // Cambiado a UserId
                                    w.Status.Equals("PENDIENTE"));
            var itemsCarrito = items.ToList();
            var total = itemsCarrito.Sum(c => c.Cantidad * c.Precio);

            dynamic model = new ExpandoObject();
            model.montoTotal = total;
            model.elementosCarrito = itemsCarrito;
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Add(int? id)
        {
            _logger.LogInformation("Iniciando el método Add.");

            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("El usuario no está autenticado.");
                ViewData["Message"] = "Por favor debe loguearse antes de agregar un producto";
                return RedirectToAction("Index", "Catalogo");
            }

            var userId = _userManager.GetUserId(User); // Obtiene el ID del usuario autenticado
            if (userId == null)
            {
                _logger.LogError("No se pudo obtener el ID del usuario.");
                ViewData["Message"] = "Hubo un problema al identificar al usuario. Intente nuevamente.";
                return RedirectToAction("Index", "Catalogo");
            }

            _logger.LogInformation("Usuario autenticado con ID: {UserId}", userId);

            var producto = await _context.DbSetProducto.FindAsync(id);
            if (producto == null)
            {
                _logger.LogWarning("El producto con ID {ProductId} no existe.", id);
                return NotFound("El producto no existe.");
            }

            _logger.LogInformation("Producto encontrado: {ProductName}, Precio: {ProductPrice}", producto.Name, producto.Price);

            var proforma = new PreOrden
            {
                Producto = producto,
                Precio = producto.Price,
                Cantidad = 1,
                UserId = userId, // Asigna el ID del usuario
                Status = "PENDIENTE"
            };

            _context.Add(proforma);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Producto agregado al carrito correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el producto en el carrito.");
                throw;
            }

            ViewData["Message"] = "Se agregó al carrito";
            return RedirectToAction("Index", "Catalogo");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var preorden = await _context.DbSetPreOrden.FindAsync(id);
            if (preorden != null)
            {
                _context.DbSetPreOrden.Remove(preorden);
                await _context.SaveChangesAsync();
                ViewData["Message"] = "Se elimino del carrito";
                _logger.LogInformation("Se elimino un producto del carrito");
            }
            return RedirectToAction("Index", "Carrito");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var itemPreOrden = await _context.DbSetPreOrden.FindAsync(id);
            if (itemPreOrden == null)
            {
                return NotFound();
            }
            return View(itemPreOrden);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cantidad,Precio,UserName")] PreOrden itemCarrito)
        {
            if (id != itemCarrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemCarrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DbSetPreOrden.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(itemCarrito);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}