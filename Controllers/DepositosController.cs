using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models.Entities;

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

        // GET: Depositos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Depositos.ToListAsync());
        }

        // GET: Depositos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposito = await _context.Depositos
                .FirstOrDefaultAsync(m => m.id == id);
            if (deposito == null)
            {
                return NotFound();
            }

            return View(deposito);
        }

        // GET: Depositos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Depositos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Ubicacion,id,Activo,CreatedAt,UpdatedAt")] Deposito deposito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deposito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deposito);
        }

        // GET: Depositos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposito = await _context.Depositos.FindAsync(id);
            if (deposito == null)
            {
                return NotFound();
            }
            return View(deposito);
        }

        // POST: Depositos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Nombre,Ubicacion,id,Activo,CreatedAt,UpdatedAt")] Deposito deposito)
        {
            if (id != deposito.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deposito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositoExists(deposito.id))
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
            return View(deposito);
        }

        // GET: Depositos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposito = await _context.Depositos
                .FirstOrDefaultAsync(m => m.id == id);
            if (deposito == null)
            {
                return NotFound();
            }

            return View(deposito);
        }

        // POST: Depositos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deposito = await _context.Depositos.FindAsync(id);
            if (deposito != null)
            {
                _context.Depositos.Remove(deposito);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepositoExists(int id)
        {
            return _context.Depositos.Any(e => e.id == id);
        }
    }
}
