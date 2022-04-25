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

namespace EmpresariosConLiderazgo.Controllers
{
    public class User_LogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public User_LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User_Logs
        public async Task<IActionResult> Index()
        {
            return View(await _context.User_Logs.ToListAsync());
        }

        // GET: User_Logs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_Logs = await _context.User_Logs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_Logs == null)
            {
                return NotFound();
            }

            return View(user_Logs);
        }

        // GET: User_Logs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User_Logs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,User,Action,Notes,Id,Name")] Logs user_Logs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user_Logs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user_Logs);
        }

        // GET: User_Logs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_Logs = await _context.User_Logs.FindAsync(id);
            if (user_Logs == null)
            {
                return NotFound();
            }
            return View(user_Logs);
        }

        // POST: User_Logs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Date,User,Action,Notes,Id,Name")] Logs user_Logs)
        {
            if (id != user_Logs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user_Logs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!User_LogsExists(user_Logs.Id))
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
            return View(user_Logs);
        }

        // GET: User_Logs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_Logs = await _context.User_Logs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_Logs == null)
            {
                return NotFound();
            }

            return View(user_Logs);
        }

        // POST: User_Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user_Logs = await _context.User_Logs.FindAsync(id);
            _context.User_Logs.Remove(user_Logs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool User_LogsExists(int id)
        {
            return _context.User_Logs.Any(e => e.Id == id);
        }

    }
}
