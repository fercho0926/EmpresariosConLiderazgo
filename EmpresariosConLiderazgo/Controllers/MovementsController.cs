using EmpresariosConLiderazgo.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmpresariosConLiderazgo.Controllers
{
    public class MovementsController : Controller
    {
        private readonly ApplicationDbContext _context;


        public MovementsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AdminCashOutRequest()
        {

            var result = _context.Movements
                   .Join(_context.Balance,
                   Movements => Movements.IdBalanceProduct,
                   Balance => Balance.Id,
                   (movement, balance) => new
                   {
                       User = balance.UserApp,
                       Product = balance.Product,
                       Available = balance.BalanceAvailable
                   }

                   ).ToList();







            return View(result);
        }

    }
}
