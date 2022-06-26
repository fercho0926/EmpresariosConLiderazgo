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
using EmpresariosConLiderazgo.Utils;
using Microsoft.AspNetCore.Authorization;

namespace EmpresariosConLiderazgo.Controllers
{
    [Authorize]
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

        //// GET: Balance/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var balance = await _context.Balance
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (balance == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(balance);
        //}

        //// GET: Balance/Create

        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Balance/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UserApp,Product,BalanceAvailable,LastMovement,InitialDate,EndlDate,Id,Name")] Balance balance)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(balance);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(balance);
        //}

        //// GET: Balance/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var balance = await _context.Balance.FindAsync(id);
        //    if (balance == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(balance);
        //}

        //// POST: Balance/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("UserApp,Product,BalanceAvailable,LastMovement,InitialDate,EndlDate,Id,Name")] Balance balance)
        //{
        //    if (id != balance.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(balance);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BalanceExists(balance.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(balance);
        //}

        //// GET: Balance/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var balance = await _context.Balance
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (balance == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(balance);
        //}

        //// POST: Balance/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var balance = await _context.Balance.FindAsync(id);
        //    _context.Balance.Remove(balance);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool BalanceExists(int id)
        //{
        //    return _context.Balance.Any(e => e.Id == id);
        //}


        public async Task<IActionResult> BalanceByMail(string mail)
        {
            if (mail == null)
            {
                RedirectToPage("Error");
            }

            if (User.Identity?.Name != mail)
            {
                return NotFound();
            }

            var movementsByMail = _context.Balance.Where(x => x.UserApp == mail).OrderByDescending(x => x.Id).ToList();

            if (movementsByMail.Count() == 0)
            {
                RedirectToPage("Error");
            }

            return View(movementsByMail);
        }


        public async Task<IActionResult> Movements(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = _context.Balance.Where(b => b.Id == id)
                .Include(x => x.MovementsByBalance).ToList();
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        public async Task<IActionResult> CashOut(int? id)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CashOut(int id,
            [Bind("UserApp,BalanceAvailable,Id,CashOut,Name,Product")]
            Balance balance)
        {
            if (id != balance.Id)
            {
                return NotFound();
            }

            if (balance.CashOut > balance.BalanceAvailable)
            {
                TempData["AlertMessage"] =
                    $"El valor del retiro por  $ {balance.CashOut}, supera el Saldo disponible : $ {balance.BalanceAvailable}";
                return View(balance);
            }

            if (balance.CashOut == 0)
            {
                TempData["AlertMessage"] =
                    $"El Valor a retirar debe ser superior a $0";
                return View(balance);
            }

            try
            {
                CreateMovement(balance.Id, "Solicitud de retiro", balance.BalanceAvailable, balance.CashOut,
                    Utils.EnumStatus.Pendiente);


                //MovementsByBalance movement = new MovementsByBalance();
                //movement.BalanceId = balance.Id;
                //movement.DateMovement = DateTime.Now;
                //movement.Name = "Solicitud Retiro";
                //movement.BalanceBefore = balance.BalanceAvailable;
                //movement.CashOut = balance.CashOut;
                //movement.BalanceAfter = balance.BalanceAvailable - balance.CashOut;
                //movement.status = Utils.EnumStatus.Pendiente;

                //balance.LastMovement = DateTime.Now;
                //balance.BalanceAvailable = balance.BalanceAvailable - balance.CashOut;

                //_context.MovementsByBalance.Add(movement);
                //_context.Update(balance);

                //await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!BalanceExists(balance.Id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }

            TempData["AlertMessage"] =
                $"Se registro la solicitud de retiro del producto {balance.Product}, por valor de $ {balance.CashOut} El desembolso se realiza el dia MARTES";
            return RedirectToAction("Index", "Home");

            return View(balance);
        }


        public async Task<IActionResult> AdminCashOutRequest()
        {
            var balancerepo = _context.Balance
                .Include(x => x.MovementsByBalance).ToList();


            return View(balancerepo);
        }


        public async Task<IActionResult> Packages()
        {
            return View();
        }

        //public async Task<IActionResult> BuyById(int? id)
        //{
        //    var NewProduct = new Balance()
        //    {
        //        UserApp = User.Identity?.Name,
        //        Name = "BASIC",
        //        Product = id.ToString(), //CAMBIAR
        //        BalanceAvailable = 1,
        //        Currency = EnumCurrencies.Peso_Colombiano,
        //        CashOut = 0,
        //        LastMovement = DateTime.Now,
        //        InitialDate = DateTime.Now,
        //        EndlDate = DateTime.Now,
        //    };
        //    _context.Add(NewProduct);

        //    await _context.SaveChangesAsync();

        //    return View();
        //}


        public async Task<IActionResult> CreateProduct()
        {
            var NewProduct = new Balance()
            {
                UserApp = User.Identity?.Name,
                Name = HttpContext.Request.Form["category"],
                Product = HttpContext.Request.Form["category"],
                BalanceAvailable = float.Parse(HttpContext.Request.Form["amount"]),
                Currency = EnumCurrencies.Peso_Colombiano,
                CashOut = 0,
                LastMovement = DateTime.Now,
                InitialDate = DateTime.Now,
                EndlDate = DateTime.Now,
                StatusBalance = EnumStatusBalance.PENDIENTE,
                Contract = false
            };
            _context.Add(NewProduct);

            await _context.SaveChangesAsync();
            var productId = await _context.Balance.SingleAsync(x => x.UserApp == NewProduct.UserApp &&
                                                                    x.InitialDate == NewProduct.InitialDate);


            CreateMovement(productId.Id, "Creacion Inicial", productId.BalanceAvailable, productId.CashOut,
                Utils.EnumStatus.creacion);

            TempData["AlertMessage"] =
                $"Se realizo la creacion del nuevo producto  {NewProduct.Product}, por valor de $ {NewProduct.BalanceAvailable}, Esta Inversión entra en un proceso de verificación, por lo cual  debe hacer la consignacion o transferencia del valor y posteriormente se le enviara a su correo el contrato para que relice la firma y pueda ser activada, hasta que esto no ocurra su Inversión no empezara a generar dividendos";

            return RedirectToAction("BalanceByMail", "Balance", new { @mail = User.Identity?.Name });
        }


        private void CreateMovement(int productID, string action, float balanceAvailable, float cashOut,
            Utils.EnumStatus status)
        {
            var movement = new MovementsByBalance
            {
                BalanceId = productID,
                DateMovement = DateTime.Now,
                Name = action,
                BalanceBefore = balanceAvailable,
                CashOut = cashOut,
                BalanceAfter = balanceAvailable - cashOut,
                status = status
            };
            _context.MovementsByBalance.Add(movement);
            _context.SaveChangesAsync();
        }
    }
}