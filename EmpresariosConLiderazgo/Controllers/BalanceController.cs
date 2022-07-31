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

            var movements =
                _context.MovementsByBalance.Where(
                    x => x.BalanceId == id && x.status == EnumStatus.PendienteDeAprobacion);

            if (movements.Count() > 0)
            {
                TempData["AlertMessage"] =
                    $"Ya se tiene una retiro en curso, cuando este sea aprobado, puede hacer uno nuevo";
                return View(balance);
            }


            try
            {
                CreateMovement(balance.Id, "Solicitud de retiro", balance.BalanceAvailable, balance.CashOut,
                    Utils.EnumStatus.PendienteDeAprobacion);
            }


            catch (DbUpdateConcurrencyException)
            {
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
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            return View();
        }


        public async Task<IActionResult> CreateProduct()
        {
            var NewProduct = new Balance()
            {
                UserApp = User.Identity?.Name,
                Name = HttpContext.Request.Form["category"],
                Product = HttpContext.Request.Form["category"],
                BalanceAvailable = decimal.Parse(HttpContext.Request.Form["amount"]),
                BaseBalanceAvailable = decimal.Parse(HttpContext.Request.Form["amount"]),
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


        public void CreateMovement(int productID, string action, decimal balanceAvailable, decimal cashOut,
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
            _context.SaveChanges();
        }


        public async Task<IActionResult> ApproveInvestments()
        {
            var recordsToApprove = await _context.Balance
                .Where(x => x.StatusBalance == EnumStatusBalance.PENDIENTE)
                .ToListAsync();
            return View(recordsToApprove);
        }


        public async Task<IActionResult> ApproveInvestmentById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.Balance
                .SingleOrDefaultAsync(b => b.Id == id);
            result.StatusBalance = EnumStatusBalance.APROBADO;
            result.InitialDate = DateTime.Now;
            result.EndlDate = DateTime.Now.AddDays(30);
            _context.SaveChanges();

            CreateMovement(result.Id, "Aprobado por el Administrador", result.BalanceAvailable, result.CashOut,
                Utils.EnumStatus.AprovadoParaTransacciones);


            TempData["AlertMessage"] =
                $"Se ha aprobado El valor de la inversion por valor de $ {result.BalanceAvailable}, para el usuario ${result.UserApp}";
            return RedirectToAction("ApproveInvestments", "Balance");
        }


        public async Task<IActionResult> RejectInvestmentById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.Balance
                .SingleOrDefaultAsync(b => b.Id == id);

            result.StatusBalance = EnumStatusBalance.RECHAZADO;
            result.InitialDate = DateTime.Now;
            result.EndlDate = DateTime.Now;
            _context.SaveChanges();

            CreateMovement(result.Id, "Rechazado por el Administrador", result.BalanceAvailable, result.CashOut,
                Utils.EnumStatus.Rechazado);


            TempData["AlertMessage"] =
                $"Se ha Rechazado la inversion por valor de $ {result.BalanceAvailable}, para el usuario ${result.UserApp}";
            return RedirectToAction("ApproveInvestments", "Balance");
        }


        public IActionResult ApproveCashOut()
        {
            var records = from m in _context.MovementsByBalance
                join b in _context.Balance on m.BalanceId equals b.Id
                where (m.status == EnumStatus.PendienteDeAprobacion)
                select new MovementBalance
                {
                    BalanceId = b.Id,
                    MovementId = m.Id,
                    UserApp = b.UserApp,
                    Product = b.Product,
                    BalanceAvailable = b.BalanceAvailable,
                    BaseBalanceAvailable = b.BaseBalanceAvailable,
                    Profit = b.Profit,
                    DateMovement = m.DateMovement,
                    CashOut = m.CashOut,
                    BalanceAfter = m.BalanceAfter,
                    Status = m.status
                };


            return View(records);
        }

        public async Task<IActionResult> ApproveCashOutById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var movement = await _context.MovementsByBalance
                .SingleOrDefaultAsync(b => b.Id == id);


            var balance = await _context.Balance
                .SingleOrDefaultAsync(b => b.Id == movement.BalanceId);


            balance.LastMovement = DateTime.Now;
            balance.BalanceAvailable -= movement.CashOut;


            balance.BaseBalanceAvailable -= movement.CashOut;
            if (balance.BalanceAvailable == 0)
            {
                balance.StatusBalance = EnumStatusBalance.FINALIZADO;
                balance.BaseBalanceAvailable = 0;
            }


            movement.status = EnumStatus.RetiroAprobado;


            _context.SaveChanges();

            CreateMovement(balance.Id, $"Retiro Aprobado - {movement.Id} ", balance.BalanceAvailable, balance.CashOut,
                Utils.EnumStatus.RetiroAprobado);


            TempData["AlertMessage"] =
                $"Se ha aprobado el retiro  para el usuario ${balance.UserApp}";
            return RedirectToAction("ApproveCashOut", "Balance");
        }

        public async Task<IActionResult> RejectCashOutById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var movement = await _context.MovementsByBalance
                .SingleOrDefaultAsync(b => b.Id == id);


            var balance = await _context.Balance
                .SingleOrDefaultAsync(b => b.Id == movement.BalanceId);


            balance.LastMovement = DateTime.Now;

            movement.status = EnumStatus.Rechazado;


            _context.SaveChanges();

            CreateMovement(balance.Id, $"Retiro Rechazado - {movement.Id} ", balance.BalanceAvailable, balance.CashOut,
                Utils.EnumStatus.Rechazado);


            TempData["AlertMessage"] =
                $"Se ha Rechazado el retiro  para el usuario ${balance.UserApp}";
            return RedirectToAction("ApproveCashOut", "Balance");
        }
    }
}