using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models.ViewModels;

namespace SistemaStock.Controllers
{
    public class StockController : Controller
    {

        private readonly AppDbContext _context;

        public StockController(AppDbContext context)
        {
            _context = context;
        }

        #region Index
        // GET: StockController
        public async Task<IActionResult> Index(int? idProducto, int? idDeposito, int? idCategoria)
        {
            var productos = _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Stocks)
            .ThenInclude(s => s.Deposito)
            .Where(p => p.Activo);
            if (idProducto.HasValue)
                productos = productos.Where(p => p.id == idProducto.Value);
            if (idCategoria.HasValue)
                productos = productos.Where(p => p.idCategoria == idCategoria.Value);
            if (idDeposito.HasValue)
                productos = productos.Where(p => p.Stocks.Any(s => s.idDeposito == idDeposito.Value));
            await LlenarFiltros();
            var lista = await productos.OrderBy(p => p.Nombre).ToListAsync();
            var model = lista.Select(p => new StockProductoViewModel
            {
                id = p.id,
                Nombre = p.Nombre,
                CategoriaNombre = p.Categoria.Nombre,
                Depositos = p.Stocks
                    .Where(s => !idDeposito.HasValue || s.idDeposito == idDeposito.Value)
                    .Select(s => new StockDepositoViewModel
                    {
                        DepositoNombre = s.Deposito!.Nombre,
                        Cantidad = s.Cantidad
                    }).ToList()
            }).ToList();
            return View(model);
        }
        #endregion

        #region Funciones
        private async Task LlenarFiltros()
        {
            ViewBag.Productos = new SelectList(await _context.Productos.OrderBy(p => p.Nombre).ToListAsync(), "id", "Nombre");
            ViewBag.Depositos = new SelectList(await _context.Depositos.OrderBy(d => d.Nombre).ToListAsync(), "id", "Nombre");
            ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "id", "Nombre");
        }
        #endregion

    }
}