using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models.Entities;
using SistemaStock.Models.ViewModels;

namespace SistemaStock.Controllers;

[Authorize(Roles = "Administrador,Encargado")]
public class ProductoController : Controller
{
    private readonly AppDbContext _context;
    public ProductoController(AppDbContext context)
    {
        _context = context;
    }
    // GET: PRODUCTOS nuevos
    public async Task<IActionResult> Index(string buscar, int? buscarCategoria)
    {
        var productos = _context.Productos
        .Include(p => p.Categoria)
        .AsQueryable();
        if (!string.IsNullOrEmpty(buscar))
            productos = productos.Where(p => p.Nombre.Contains(buscar) ||
            p.Codigo!.Contains(buscar));
        if (buscarCategoria.HasValue)
            productos = productos.Where(p => p.idCategoria == buscarCategoria.Value);
        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "id",
        "Nombre");
        // Mapeo manual de Lista de Entidades -> Lista de ViewModels
        var listaViewModels = productos.Select(p => new ProductoViewModel
        {
            id = p.id,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            PrecioCosto = p.PrecioCosto,
            PrecioVenta = p.PrecioVenta,
            StockMinimo = p.StockMinimo,
            CategoriaNombre = p.Categoria!.Nombre,
            UnidadDeMedida = p.UnidadDeMedida
        }).ToList();
        return View(listaViewModels);
    }
    // GET: PRODUCTOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {

        return NotFound();
        }
        var producto = await _context.Productos.Include(p => p.Categoria)
        .FirstOrDefaultAsync(m => m.id == id);
        if (producto == null)
        {
            return NotFound();
        }
        // Mapeo manual de Entidad -> ViewModel
        var model = new ProductoViewModel
        {
            id = producto.id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioCosto = producto.PrecioCosto,
            PrecioVenta = producto.PrecioVenta,
            StockMinimo = producto.StockMinimo,
            CategoriaNombre = producto.Categoria?.Nombre,
            UnidadDeMedida = producto.UnidadDeMedida,
            Activo = producto.Activo,
            CreatedAt = producto.CreatedAt,
            UpdatedAt = producto.UpdatedAt,
        };
        return View(model);
    }

    // GET: PRODUCTOS/Create
    public async Task<IActionResult> CreateAsync()
    {
        var model = new ProductoViewModel
        {
            Categorias = await ObtenerCategorias()
        };
        return View(model);
    }
    private async Task<List<SelectListItem>> ObtenerCategorias(int? idSeleccionado = null)
    {
        var categorias = await _context.Categorias
        .Where(c => c.Activo)
        .AsNoTracking()
        .OrderBy(c => c.Nombre)
        .ToListAsync();
        return new SelectList(categorias, "id", "Nombre", idSeleccionado).ToList();
    }
    // POST: PRODUCTOS/Create

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductoViewModel model)
    {
        if (ModelState.IsValid)
        {
            var nombre = model.Nombre.ToLower();
            var codigo = model.Codigo!.ToLower();
            bool existeNombre = await _context.Productos.AnyAsync(p => p.Nombre.ToLower() == nombre);
            bool existeCodigo = await _context.Productos.AnyAsync(p => p.Codigo!.ToLower() == codigo);
            if (existeNombre) ModelState.AddModelError("Nombre", "Ya existe un producto con este nombre.");
            if (existeCodigo) ModelState.AddModelError("Codigo", "Ya existe un producto con este código.");
            if (existeNombre || existeCodigo)
            {
                model.Categorias = await ObtenerCategorias(model.idCategoria);
                return View(model);
            }
            var producto = new Producto
            {
                Codigo = model.Codigo,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                PrecioCosto = model.PrecioCosto,
                PrecioVenta = model.PrecioVenta,
                StockMinimo = model.StockMinimo,
                idCategoria = model.idCategoria!.Value,
                UnidadDeMedida = model.UnidadDeMedida
            };
            _context.Add(producto);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Producto creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        model.Categorias = await ObtenerCategorias();
        return View(model);
    }
    // GET: PRODUCTOS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return NotFound();
        }
        // Mapeo manual de Entidad -> ViewModel (para rellenar el formulario)
        var model = new ProductoViewModel
        {
            id = producto.id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioCosto = producto.PrecioCosto,
            PrecioVenta = producto.PrecioVenta,
            StockMinimo = producto.StockMinimo,
            idCategoria = producto.idCategoria,
            UnidadDeMedida = producto.UnidadDeMedida
        };
        model.Categorias = await ObtenerCategorias(producto.idCategoria);
        return View(model);
    }
    // POST: PRODUCTOS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, ProductoViewModel model)
    {
        if (id != model.id)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            var nombre = model.Nombre.ToLower();
            var codigo = model.Codigo!.ToLower();
            bool existeNombre = await _context.Productos.AnyAsync(p => p.id != model.id && p.Nombre.ToLower() == nombre);
            bool existeCodigo = await _context.Productos.AnyAsync(p => p.id != model.id && p.Codigo!.ToLower() == codigo);
            if (existeNombre) ModelState.AddModelError("Nombre", "Ya existe un producto con este nombre.");
            if (existeCodigo) ModelState.AddModelError("Codigo", "Ya existe un producto con este código.");
            if (existeNombre || existeCodigo)
            {
                model.Categorias = await ObtenerCategorias(model.idCategoria);
                return View(model);
            }
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                producto.Codigo = model.Codigo;
                producto.Nombre = model.Nombre;
                producto.Descripcion = model.Descripcion;
                producto.PrecioCosto = model.PrecioCosto;
                producto.PrecioVenta = model.PrecioVenta;
                producto.StockMinimo = model.StockMinimo;
                producto.idCategoria = model.idCategoria!.Value;
                producto.UnidadDeMedida = model.UnidadDeMedida;
                producto.Activo = model.Activo;
                producto.UpdatedAt = DateTime.UtcNow;
                _context.Update(producto);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Producto actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(model.id))
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
        model.Categorias = await ObtenerCategorias(model.idCategoria);
        return View(model);
    }
    // GET: PRODUCTOS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var producto = await _context.Productos
        .Include(p => p.Categoria)
        .FirstOrDefaultAsync(m => m.id == id);
        if (producto == null)
        {
            return NotFound();
        }

        // Mapeo manual de Entidad -> ViewModel
        var model = new ProductoViewModel
        {
            id = producto.id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioCosto = producto.PrecioCosto,
            PrecioVenta = producto.PrecioVenta,
            StockMinimo = producto.StockMinimo,
            idCategoria = producto.idCategoria,
            CategoriaNombre = producto.Categoria?.Nombre,
            UnidadDeMedida = producto.UnidadDeMedida
        };
        return View(model);
    }
    // POST: PRODUCTOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var producto = await _context.Productos.Include(p => p.Movimientos).FirstOrDefaultAsync(p => p.id == id);
        if (producto != null)
        {
            if (producto.Movimientos.Any())
            {
                TempData["Error"] = $"No se puede eliminar el producto '{producto.Nombre}' porque tiene movimientos asociados.";
                return RedirectToAction(nameof(Index));
            }
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Producto eliminado con éxito.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ProductoExists(int? id)
    {
        return _context.Productos.Any(e => e.id == id);
    }
}