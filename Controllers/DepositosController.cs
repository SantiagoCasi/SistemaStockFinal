using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models.Entities;
using SistemaStock.Models.ViewModels;

namespace SistemaStock.Controllers
{
    [Authorize(Roles = "Administrador,Encargado")]
    public class DepositosController : Controller
    {
        private readonly AppDbContext _context;

        public DepositosController(AppDbContext context)
        {
            _context = context;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var depositos = await _context.Depositos.ToListAsync();
            return View(depositos.Select(AViewModel).ToList());
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var deposito = await _context.Depositos.FirstOrDefaultAsync(d => d.id == id);
            if (deposito == null) return NotFound();
            return View(AViewModel(deposito));
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View(new DepositoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepositoViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool existeDeposito = await _context.Depositos.AnyAsync(d => d.Nombre.ToLower() == model.Nombre.ToLower());
                if (existeDeposito)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un depósito con este nombre.");
                    return View(model);
                }
                var nuevoDeposito = new Deposito
                {
                    Nombre = model.Nombre,
                    Ubicacion = model.Ubicacion,
                    Activo = model.Activo
                };
                _context.Depositos.Add(nuevoDeposito);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "¡El depósito se ha creado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var deposito = await _context.Depositos.FindAsync(id);
            if (deposito == null) return NotFound();
            return View(AViewModel(deposito));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, DepositoViewModel model)
        {
            if (id != model.id) return NotFound();
            if (ModelState.IsValid)
            {
                bool existeDeposito = await _context.Depositos.AnyAsync(d => d.id != model.id && d.Nombre.ToLower() == model.Nombre.ToLower());
                if (existeDeposito)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un depósito con este nombre.");
                    return View(model);
                }
                var deposito = await _context.Depositos.FindAsync(id);
                if (deposito == null) return NotFound();
                deposito.Nombre = model.Nombre;
                deposito.Ubicacion = model.Ubicacion;
                deposito.Activo = model.Activo;
                deposito.UpdatedAt = DateTime.Now;
                _context.Depositos.Update(deposito);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "¡El depósito se ha modificado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var deposito = await _context.Depositos.FindAsync(id);
            if (deposito == null) return NotFound();
            return View(AViewModel(deposito));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var deposito = await _context.Depositos
                .Include(d => d.MovimientosOrigen)
                .Include(d => d.MovimientosDestino)
                .FirstOrDefaultAsync(d => d.id == id);
            if (deposito != null)
            {
                if (deposito.MovimientosOrigen.Any() || deposito.MovimientosDestino.Any())
                {
                    TempData["Error"] = $"No se puede eliminar el depósito '{deposito.Nombre}' porque tiene movimientos asociados.";
                    return RedirectToAction(nameof(Index));
                }
                _context.Depositos.Remove(deposito);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Depósito eliminado con éxito.";
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Funciones
        private static DepositoViewModel AViewModel(Deposito deposito)
        {
            return new DepositoViewModel
            {
                id = deposito.id,
                Nombre = deposito.Nombre,
                Ubicacion = deposito.Ubicacion,
                Activo = deposito.Activo,
                CreatedAt = deposito.CreatedAt,
                UpdatedAt = deposito.UpdatedAt
            };
        }
        #endregion
    }
}
