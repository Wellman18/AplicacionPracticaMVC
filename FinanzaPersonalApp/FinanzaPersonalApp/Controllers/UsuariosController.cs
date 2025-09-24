using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinanzaPersonalApp.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FinanzaPersonalApp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ConnectionManagerDbContext _context;
        IConfiguration _configuration;
        private readonly HttpClient httpClient;
        IEnumerable<Usuario> listaUsuarios = Enumerable.Empty<Usuario>();

        public UsuariosController(ConnectionManagerDbContext context,IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            this.httpClient = httpClient;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var url = _configuration.GetSection("CustomValues")
                                    .Get<List<CustomValues>>()
                                    .FirstOrDefault(x=> x.key== "ObtenerUsuario")?.value;

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                listaUsuarios = JsonSerializer.Deserialize<List<Usuario>>(content,options);

                return listaUsuarios != null
                                        ? View(listaUsuarios)
                                        : Problem("No se pudieron deserializar los usuarios.");

            }

            return Problem("Error al obtener datos de la API.");
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Correo")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var url = _configuration.GetSection("CustomValues")
                        .Get<List<CustomValues>>()
                        .FirstOrDefault(x => x.key == "CrearUsuario")?.value;

                var response = await httpClient.PostAsJsonAsync(url, usuario);

                if (response.IsSuccessStatusCode)
                {
                    //var content = await response.Content.ReadAsStringAsync();

                    //var options = new JsonSerializerOptions
                    //{
                    //    PropertyNameCaseInsensitive = true
                    //};

                    //listaUsuarios = JsonSerializer.Deserialize<List<Usuario>>(content, options);

                    //return listaUsuarios != null
                    //                        ? View(listaUsuarios)
                    //                        : Problem("No se pudieron deserializar los usuarios.");

                    //_context.Add(usuario);
                    //await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));




                }


            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Correo")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'ConnectionManagerDbContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
