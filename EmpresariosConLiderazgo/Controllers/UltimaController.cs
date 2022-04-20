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
    public class UltimaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UltimaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ultima
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ultima.ToListAsync());
        }

        // GET: Ultima/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ultima = await _context.Ultima
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ultima == null)
            {
                return NotFound();
            }

            return View(ultima);
        }

        // GET: Ultima/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ultima/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,Date")] Ultima ultima)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ultima);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ultima);
        }

        // GET: Ultima/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ultima = await _context.Ultima.FindAsync(id);
            if (ultima == null)
            {
                return NotFound();
            }
            return View(ultima);
        }

        // POST: Ultima/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,Date")] Ultima ultima)
        {
            if (id != ultima.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ultima);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UltimaExists(ultima.Id))
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
            return View(ultima);
        }

        // GET: Ultima/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ultima = await _context.Ultima
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ultima == null)
            {
                return NotFound();
            }

            return View(ultima);
        }

        // POST: Ultima/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ultima = await _context.Ultima.FindAsync(id);
            _context.Ultima.Remove(ultima);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UltimaExists(int id)
        {
            return _context.Ultima.Any(e => e.Id == id);
        }
    }
}
