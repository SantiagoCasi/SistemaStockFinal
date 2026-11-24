using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models.Entities;
using SistemaStock.Models.ViewModels;

namespace SistemaStock.Controllers
{
    [Authorize(Roles = "Administrador,Encargado")]
    public class ProveedoresController : Controller
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var proveedores = await _context.Proveedores.ToListAsync();
            return View(proveedores.Select(AViewModel).ToList());
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.id == id);
            if (proveedor == null) return NotFound();
            return View(AViewModel(proveedor));
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View(new ProveedorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProveedorViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool existeProveedor = await _context.Proveedores.AnyAsync(p => p.Nombre.ToLower() == model.Nombre.ToLower());
                if (existeProveedor)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un proveedor con este nombre.");
                    return View(model);
                }
                var nuevoProveedor = new Proveedor
                {
                    Nombre = model.Nombre,
                    Contacto = model.Contacto,
                    Documento = model.Documento,
                    Telefono = model.Telefono,
                    Email = model.Email,
                    Direccion = model.Direccion,
                    Observaciones = model.Observaciones,
                    Activo = model.Activo
                };
                _context.Proveedores.Add(nuevoProveedor);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "¡El proveedor se ha creado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();
            return View(AViewModel(proveedor));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProveedorViewModel model)
        {
            if (id != model.id) return NotFound();
            if (ModelState.IsValid)
            {
                bool existeProveedor = await _context.Proveedores.AnyAsync(p => p.id != model.id && p.Nombre.ToLower() == model.Nombre.ToLower());
                if (existeProveedor)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un proveedor con este nombre.");
                    return View(model);
                }
                var proveedor = await _context.Proveedores.FindAsync(id);
                if (proveedor == null) return NotFound();
                proveedor.Nombre = model.Nombre;
                proveedor.Contacto = model.Contacto;
                proveedor.Documento = model.Documento;
                proveedor.Telefono = model.Telefono;
                proveedor.Email = model.Email;
                proveedor.Direccion = model.Direccion;
                proveedor.Observaciones = model.Observaciones;
                proveedor.Activo = model.Activo;
                proveedor.UpdatedAt = DateTime.Now;
                _context.Proveedores.Update(proveedor);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "¡El proveedor se ha modificado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();
            return View(AViewModel(proveedor));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var proveedor = await _context.Proveedores.Include(p => p.Movimientos).FirstOrDefaultAsync(p => p.id == id);
            if (proveedor != null)
            {
                if (proveedor.Movimientos.Any())
                {
                    TempData["Error"] = $"No se puede eliminar el proveedor '{proveedor.Nombre}' porque tiene movimientos asociados.";
                    return RedirectToAction(nameof(Index));
                }
                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Proveedor eliminado con éxito.";
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Funciones
        private static ProveedorViewModel AViewModel(Proveedor proveedor)
        {
            return new ProveedorViewModel
            {
                id = proveedor.id,
                Nombre = proveedor.Nombre,
                Contacto = proveedor.Contacto,
                Documento = proveedor.Documento,
                Telefono = proveedor.Telefono,
                Email = proveedor.Email,
                Direccion = proveedor.Direccion,
                Observaciones = proveedor.Observaciones,
                Activo = proveedor.Activo,
                CreatedAt = proveedor.CreatedAt,
                UpdatedAt = proveedor.UpdatedAt
            };
        }
        #endregion
    }
}
