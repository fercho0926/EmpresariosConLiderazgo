#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmpresariosConLiderazgo.Data;
using EmpresariosConLiderazgo.Models;
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
            string UserLogged = User.Identity?.Name.ToString();
            var completed = await  _context.Users_App.FirstOrDefaultAsync(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            if (mail == null)
            {
                RedirectToPage("Error");
            }

            if (User.Identity?.Name != mail)
            {
                return NotFound();
            }

            var movementsByMail = _context.Balance.Where(x => x.UserApp == mail).OrderByDescending(x => x.Id).ToList();

            if (movementsByMail.Count == 0)
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
            [Bind("UserApp,Profit,BaseBalanceAvailable,BalanceAvailable,Id,CashOut,Name,Product")]
            Balance balance)
        {
            if (id != balance.Id)
            {
                return NotFound();
            }

            if (balance.CashOut > balance.Profit)
            {
                TempData["AlertMessage"] =
                    $"El valor del retiro por  $ {balance.CashOut}, supera el Saldo disponible de Utilidad: $ {balance.Profit}";
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
                    x => x.BalanceId == id && x.status == EnumStatus.PendienteDeAprobación);

            if (movements.Count() > 0)
            {
                TempData["AlertMessage"] =
                    $"Ya se tiene una retiro en curso, cuando este sea aprobado, puede hacer uno nuevo";
                return View(balance);
            }


            try
            {
                CreateMovement(balance.Id, "Solicitud de retiro", 0, balance.CashOut,
                    Utils.EnumStatus.PendienteDeAprobación);
            }


            catch (DbUpdateConcurrencyException)
            {
            }

            TempData["AlertMessage"] =
                $"Se registró la solicitud de retiro del producto {balance.Product}, por valor de $ {balance.CashOut} El desembolso se realiza entre los días 12 al 15 de cada mes";


            string UserLogged = User.Identity?.Name.ToString();
            return RedirectToAction("BalanceByMail", "Balance", new { @mail = UserLogged });

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
            var completed = await _context.Users_App.FirstOrDefaultAsync(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            return View();
        }
        public async Task<IActionResult> EmergencyFund()
        {
            string  UserLogged = User.Identity?.Name.ToString();
            var  completed = await _context.Users_App.FirstOrDefaultAsync(m => m.AspNetUserId == UserLogged);

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


            var refer = await _context.ReferedByUser.OrderByDescending(x=>x.Date).FirstOrDefaultAsync(x =>
                x.ReferedUserId == NewProduct.UserApp && x.InvestDone == false);

            if (refer != null)
            {
                refer.InvestDone = true;

                var movement = new ReferedByUserMovement()
                {
                    ReferedByUserId = refer.Id,
                    Message = "Realizó Inversión",
                    DateMovement = DateTime.Now,
                    Status = EnumStatusBalance.PENDIENTE
                };
                _context.ReferedByUserMovement.Add(movement);
            }


            await _context.SaveChangesAsync();
            var productId = await _context.Balance.SingleAsync(x => x.UserApp == NewProduct.UserApp &&
                                                                    x.InitialDate == NewProduct.InitialDate);


            CreateMovement(productId.Id, "Creación Inicial", productId.BalanceAvailable, productId.CashOut,
                Utils.EnumStatus.creación);


            TempData["AlertMessage"] =
                $"Se realizo la creación del nuevo producto  {NewProduct.Product}, por valor de $ {NewProduct.BalanceAvailable}, Esta Inversión entra en un proceso de verificación, por lo cual  debe hacer la consignación o transferencia del valor y posteriormente se le enviara a su correo el contrato para que relice la firma y pueda ser activada, hasta que esto no ocurra su Inversión no empezara a generar dividendos";

            return RedirectToAction("BalanceByMail", "Balance", new { @mail = User.Identity?.Name });
        }


        public void CreateMovement(int productID, string action, decimal balanceAvailable, decimal cashOut,
            Utils.EnumStatus status)
        {
            decimal balanceAfter = 0;
            if (balanceAvailable != 0)
            {
                balanceAfter = balanceAvailable - cashOut;
            }


            var movement = new MovementsByBalance
            {
                BalanceId = productID,
                DateMovement = DateTime.Now,
                Name = action,
                BalanceBefore = balanceAvailable,
                CashOut = cashOut,
                BalanceAfter = balanceAfter,
                status = status
            };
            _context.MovementsByBalance.Add(movement);
            _context.SaveChanges();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveInvestments()
        {
            var recordsToApprove = await _context.Balance
                .Where(x => x.StatusBalance == EnumStatusBalance.PENDIENTE)
                .ToListAsync();
            return View(recordsToApprove);
        }

        [Authorize(Roles = "Admin")]
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
            result.EndlDate = DateTime.Now.AddYears(1);


            var refer = await _context.ReferedByUser.OrderByDescending(x => x.Date).FirstOrDefaultAsync(x =>
             x.ReferedUserId == result.UserApp && x.ApproveByAdmin == false);


          

            if (refer != null)
            {
                refer.ApproveByAdmin = true;

                var firstOperation = _context.Balance.Where(x => x.UserApp == refer.ReferedUserId).OrderBy(x => x.Id)
                    .First();

                var toReturn = new Balance_ReferenceByuser();
                toReturn.Balance = firstOperation;
                toReturn.ReferedByUser = refer;


                var movement = new ReferedByUserMovement()
                {
                    ReferedByUserId = refer.Id,
                    Message = "Se aprueba Inversión",
                    DateMovement = DateTime.Now,
                    Status = EnumStatusBalance.APROBADO
                };
                _context.ReferedByUserMovement.Add(movement);


                await _context.SaveChangesAsync();

                CreateMovement(result.Id, "Aprobado por el Administrador", result.BalanceAvailable, result.CashOut,
                    Utils.EnumStatus.AprobadoParaTransacciones);

                return View("ApproveComission", toReturn);
            }


            await _context.SaveChangesAsync();

            CreateMovement(result.Id, "Aprobado por el Administrador", result.BalanceAvailable, result.CashOut,
                Utils.EnumStatus.AprobadoParaTransacciones);


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

        [Authorize(Roles = "Admin")]
        public IActionResult ApproveCashOut()
        {
            var records = from m in _context.MovementsByBalance
                join b in _context.Balance on m.BalanceId equals b.Id
                where (m.status == EnumStatus.PendienteDeAprobación)
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

        [Authorize(Roles = "Admin")]
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


            if (movement!.CashOut > balance!.Profit)
            {
                TempData["Error"] = $"El valor a retirar es superior al disponible";
                return RedirectToAction("ApproveCashOut", "Balance");
            }


            balance.LastMovement = DateTime.Now;

            var balanceAvailableOld = balance.BalanceAvailable;
            balance.BalanceAvailable -= movement.CashOut;
            balance.Profit -= movement.CashOut;


            movement.status = EnumStatus.RetiroAprobado;


            _context.SaveChanges();

            CreateMovement(balance.Id, $"Retiro Aprobado - {movement.Id} ", balanceAvailableOld, movement.CashOut,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMoneyPerReferee(
            [Bind(
                "Id,AmountToRefer")]
            ReferedByUser ReferedByUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _context.ReferedByUser.SingleOrDefaultAsync(x => x.Id == ReferedByUser.Id);
                    result!.AmountToRefer = ReferedByUser.AmountToRefer;


                    var movement = new ReferedByUserMovement()
                    {
                        ReferedByUserId = ReferedByUser.Id,
                        Message = $"Se abona {ReferedByUser.AmountToRefer} , Disponible para retirar",
                        DateMovement = DateTime.Now,
                        Status = EnumStatusBalance.POR_RETIRAR
                    };
                    _context.ReferedByUserMovement.Add(movement);


                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }


                TempData["AlertMessage"] =
                    $"Se ha Abonado la comisión";
                return RedirectToAction("ApproveInvestments", "Balance");
            }

            return View("ApproveComission");
        }
    }
}