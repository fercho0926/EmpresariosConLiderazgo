#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpresariosConLiderazgo.Data;
using EmpresariosConLiderazgo.Models;
using Microsoft.AspNetCore.Authorization;

namespace EmpresariosConLiderazgo.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class Users_AppController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Users_AppController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users_App
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users_App.ToListAsync());
        }

        // GET: Users_App/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users_App = await _context.Users_App
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users_App == null)
            {
                return NotFound();
            }

            return View(users_App);
        }

        // GET: Users_App/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users_App/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "LastName,Identification,DateBirth,EnumCountries,City,Neighborhood,Address,phone,AspNetUserId,Id,Name")]
            Users_App users_App)
        {
            if (ModelState.IsValid)
            {
                _context.Add(users_App);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(users_App);
        }

        // GET: Users_App/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users_App = await _context.Users_App.FindAsync(id);
            if (users_App == null)
            {
                return NotFound();
            }

            return View(users_App);
        }

        // POST: Users_App/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind(
                "LastName,Identification,DateBirth,EnumCountries,City,Neighborhood,Address,phone,AspNetUserId,Id,Name")]
            Users_App users_App)
        {
            if (id != users_App.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(users_App);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Users_AppExists(users_App.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["AlertMessage"] =
                    $"Se ha realizado la actualizacion de la Informacion";
                return RedirectToAction("Index", "Home");
            }

            return View(users_App);
        }

        // GET: Users_App/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users_App = await _context.Users_App
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users_App == null)
            {
                return NotFound();
            }

            return View(users_App);
        }

        // POST: Users_App/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users_App = await _context.Users_App.FindAsync(id);
            _context.Users_App.Remove(users_App);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Users_AppExists(int id)
        {
            return _context.Users_App.Any(e => e.Id == id);
        }

        public async Task<IActionResult> EditByMail(string mail)
        {
            if (User.Identity?.Name != mail)
            {
                return NotFound();
            }

            if (mail == null)
            {
                return NotFound();
            }

            var users_App = _context.Users_App.FirstOrDefault(x => x.AspNetUserId == mail);
            if (users_App == null)
            {
                return NotFound();
            }

            return View(users_App);
        }
    }
}