using Canela.Data;
using Canela.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Canela.Controllers
{
    [Authorize(Roles = "Admin")] // Requiere rol de administrador
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                Pagos = await _context.DbSetPago.ToListAsync(),
                Usuarios = await _userManager.Users.ToListAsync()
            };
            return View(model);
        }

        // CRUD para Productos
        public IActionResult Productos()
        {
            var productos = _context.DbSetProducto.ToList();
            return View(productos);
        }

        [HttpGet]
        public IActionResult CrearProducto()
        {
            // Obtener el próximo ID disponible
            var proximoId = _context.DbSetProducto.Any() ? 
                        _context.DbSetProducto.Max(p => p.Id) + 1 : 1;
            
            return View(new Producto { Id = proximoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            // Validar si el ID ya existe
            if (_context.DbSetProducto.Any(p => p.Id == producto.Id))
            {
                ModelState.AddModelError("Id", "Este ID ya está en uso");
            }

            if (ModelState.IsValid)
            {
                producto.Status = "en oferta";
                _context.DbSetProducto.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Productos));
            }
            
            return View(producto);
        }


        [HttpGet]
        public async Task<IActionResult> EditarProducto(int id)
        {
            var producto = await _context.DbSetProducto.FindAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                producto.Status = "en oferta";
                _context.DbSetProducto.Update(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Productos));
            }
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.DbSetProducto.FindAsync(id);
            if (producto != null)
            {
                _context.DbSetProducto.Remove(producto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Productos");
        }
    }
}