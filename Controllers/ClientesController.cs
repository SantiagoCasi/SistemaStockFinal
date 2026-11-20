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
using SistemaStock.Models.ViewModels;

namespace SistemaStock.Controllers
{
    [Authorize(Roles = "Administrador,Encargado")]
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            // Traemos los clientes de la base de datos
            var clientes = await _context.Clientes.ToListAsync();
            // Mapeo manual de Lista de Entidades -> Lista de ViewModels
            var listaViewModels = clientes.Select(c => new ClienteViewModel
            {
                id = c.id,
                Nombre = c.Nombre,
                Documento = c.Documento,
                Email = c.Email,
                Telefono = c.Telefono,
                Activo = c.Activo,
                Contacto = c.Contacto,
                Direccion = c.Direccion,
                Observaciones = c.Observaciones,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt

            }).ToList();
            return View(listaViewModels);
        }




        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.id == id);
            if (cliente == null) return NotFound();

            // Mapeo manual de Entidad -> ViewModel
            var model = new ClienteViewModel
            {
                id = cliente.id,
                Nombre = cliente.Nombre,
                Documento = cliente.Documento,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Activo = cliente.Activo,
                Contacto = cliente.Contacto,
                Direccion = cliente.Direccion,
                Observaciones = cliente.Observaciones,
                CreatedAt = cliente.CreatedAt,
                UpdatedAt = cliente.UpdatedAt
            };
            return View(model);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Validar que el nombre no exista en la base de datos (ignorando mayúsculas / minúsculas)
                bool existeCliente = await _context.Clientes.AnyAsync(c => c.Nombre.ToLower() == model.Nombre.ToLower());

                if (existeCliente)
                {
                    // 2. Agregar el error al ModelState para que se muestre en la vista
                    ModelState.AddModelError("Nombre", "Ya existe un cliente con este nombre.");
                    // Retorna la misma vista con el modelo para no perder los datos ingresados
                    return View(model);
                }

                // Mapeo manual de ViewModel -> Entidad (para guardar en la DB)
                var nuevoCliente = new Cliente
                {
                    Nombre = model.Nombre,
                    Documento = model.Documento,
                    Email = model.Email,
                    Telefono = model.Telefono,
                    Activo = model.Activo,
                    Contacto = model.Contacto,
                    Direccion = model.Direccion,
                    Observaciones = model.Observaciones

                    // Nota: 'id' no se asigna porque la base de datos lo genera solo (Identity)
                };
                _context.Clientes.Add(nuevoCliente);
                await _context.SaveChangesAsync();

                // Guardamos el mensaje en TempData antes de redirigir
                TempData["Ok"] = "¡El cliente se ha creado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }









        // GET: CLIENTES/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null) return NotFound();
            // Mapeo manual de Entidad -> ViewModel (para rellenar el formulario)
            var model = new ClienteViewModel
            {
                id = cliente.id,
                Nombre = cliente.Nombre,
                Documento = cliente.Documento,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Activo = cliente.Activo,
                Contacto = cliente.Contacto,
                Direccion = cliente.Direccion,
                Observaciones = cliente.Observaciones,
                CreatedAt = cliente.CreatedAt,
                UpdatedAt = cliente.UpdatedAt
            };
            return View(model);
        }

        // POST: CLIENTES/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ClienteViewModel model)
        {
            if (id != model.id) return NotFound();
            if (ModelState.IsValid)
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null) return NotFound();
                // Mapeo manual de actualización: asignamos los datos modificados del ViewModel a la entidad rastreada
                cliente.Nombre = model.Nombre;
                cliente.Contacto = model.Contacto;
                cliente.Direccion = model.Direccion;
                cliente.Observaciones = model.Observaciones;
                cliente.Documento = model.Documento;
                cliente.Email = model.Email;
                cliente.Telefono = model.Telefono;
                cliente.Activo = model.Activo;
                cliente.UpdatedAt = DateTime.UtcNow;
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
                // Guardamos el mensaje en TempData antes de redirigir
                TempData["Ok"] = "¡El cliente se ha modificado con éxito!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: CLIENTES/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            // Mapeo manual de Entidad -> ViewModel
            var model = new ClienteViewModel
            {
                id = cliente.id,
                Nombre = cliente.Nombre,
                Documento = cliente.Documento,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Activo = cliente.Activo
            };
            return View(model);
        }

        // POST: CLIENTES/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var cliente = await _context.Clientes.Include(c =>
            c.Movimientos).FirstOrDefaultAsync(c => c.id == id);
            if (cliente != null)
            {
                // Validamos si tiene movimientos asociados
                if (cliente.Movimientos.Any())
                {
                    // Guardamos el mensaje de error en TempData
                    TempData["Error"] = $"No se puede eliminar el cliente '{cliente.Nombre}' porque tiene movimientos asociados.";
                    return RedirectToAction(nameof(Index));
                }
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                // Mensaje de éxito
                TempData["Ok"] = "Cliente eliminado con éxito.";
            }
            return RedirectToAction(nameof(Index));
        }
        private bool ClienteExists(int? id)
        {
            return _context.Clientes.Any(e => e.id == id);
        }

    }
}
