#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpresariosConLiderazgo.Data;
using EmpresariosConLiderazgo.Models.Entities;

namespace EmpresariosConLiderazgo.Controllers
{
    public class BalanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BalanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Balance
        public async Task<IActionResult> Index()
        {
            return View(await _context.Balance.ToListAsync());
        }

        // GET: Balance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balance
                .FirstOrDefaultAsync(m => m.Id == id);
            if (balance == null)
            {
                return NotFound();
            }

            return View(balance);
        }

        // GET: Balance/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Balance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserApp,Product,BalanceAvailable,LastMovement,InitialDate,EndlDate,Id,Name")] Balance balance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(balance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(balance);
        }

        // GET: Balance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balance.FindAsync(id);
            if (balance == null)
            {
                return NotFound();
            }
            return View(balance);
        }

        // POST: Balance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserApp,Product,BalanceAvailable,LastMovement,InitialDate,EndlDate,Id,Name")] Balance balance)
        {
            if (id != balance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(balance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BalanceExists(balance.Id))
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
            return View(balance);
        }

        // GET: Balance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balance
                .FirstOrDefaultAsync(m => m.Id == id);
            if (balance == null)
            {
                return NotFound();
            }

            return View(balance);
        }

        // POST: Balance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var balance = await _context.Balance.FindAsync(id);
            _context.Balance.Remove(balance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BalanceExists(int id)
        {
            return _context.Balance.Any(e => e.Id == id);
        }




        public async Task<IActionResult> BalanceByMail_OLD(string mail)
        {

            if (User.Identity?.Name != mail)
            {
                return NotFound();
            }

            if (mail == null)
            {
                return NotFound();
            }

            var result = _context.Balance.FirstOrDefault(x => x.UserApp == mail);



            //if (balance == null)
            //{
            //    return NotFound();
            //}
            return View(result);

        }


        public async Task<IActionResult> BalanceByMail(string mail)
        {
            if (mail == null)
            {
                return NotFound();
            }

            if (User.Identity?.Name != mail)
            {
                return NotFound();
            }
            var TotalBalance = _context.Balance.ToList().Where(x => x.UserApp == mail);

            if (TotalBalance.Count() == 0)
            {
                return NotFound();
            }
            return View(TotalBalance);
        }

        // GET: Balance/Details/5
        public async Task<IActionResult> Movements(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balance
                .FirstOrDefaultAsync(m => m.Id == id);

            var movements = _context.Movements
                .Where(b => b.IdBalanceProduct == id)
                .ToList();

            ViewBag.Movements = movements;




            if (balance == null)
            {
                return NotFound();
            }

            return View(balance);
        }


    }
}
