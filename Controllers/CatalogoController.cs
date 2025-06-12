using Canela.Models;
using Canela.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Canela.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ProductoService _productoService;

        public CatalogoController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var productos = await _productoService.GetAll();
                return View(productos ?? new List<Producto>());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar productos: {ex.Message}");
                return View(new List<Producto>());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}