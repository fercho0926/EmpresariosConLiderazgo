using EmpresariosConLiderazgo.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using EmpresariosConLiderazgo.Models.Entities;
using EmpresariosConLiderazgo.Services;
using EmpresariosConLiderazgo.Utils;

namespace EmpresariosConLiderazgo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CronController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICloudwatchLogs _cloudwatchLogs;

        public CronController(ApplicationDbContext context, ICloudwatchLogs cloudwatchLogs)
        {
            _context = context;
            _cloudwatchLogs = cloudwatchLogs;
        }

        [HttpGet]
        public IActionResult ExecuteCron()
        {
            var records = _context.Balance.Where(x => x.StatusBalance == Utils.EnumStatusBalance.APROBADO).ToList();

            foreach (var record in records)
            {
                double Fee = 0.0;
                switch (record.Product)
                {
                    case "PLUS":
                        Fee = 0.00333;
                        break;
                    case "STAR":
                        Fee = 0.00333;
                        break;
                    case "VIP":
                        Fee = 0.00333;
                        break;
                    case "ELITE":
                        Fee = 0.00333;
                        break;
                }


                decimal profit = (record.BaseBalanceAvailable * Convert.ToDecimal(Fee));

                var oldBalance = record.BalanceAvailable;
                record.BalanceAvailable += profit;
                record.Profit += profit;


                var movement = new MovementsByBalance
                {
                    BalanceId = record.Id,
                    DateMovement = DateTime.Now,
                    Name = $"Abono Intereses {String.Format("{0:0.##}", profit)} ",
                    BalanceBefore = oldBalance,
                    CashOut = 0,
                    BalanceAfter = record.BalanceAvailable
                };
                _context.MovementsByBalance.Add(movement);
            }

            _cloudwatchLogs.InsertLogs("a", "b", "c");
            _context.SaveChanges();
            return Ok();
        }
    }
}