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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.DbSetProducto.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Productos");
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
        public async Task<IActionResult> EditarProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.DbSetProducto.Update(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Productos");
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