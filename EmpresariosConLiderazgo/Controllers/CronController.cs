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
                    case "INICIO":
                        Fee = DailyFeeCalculator(4);
                        break;
                    case "PLUS":
                        Fee = DailyFeeCalculator(5);
                        break;
                    case "STAR":
                        Fee = DailyFeeCalculator(6.5);
                        break;
                    case "ASOCIADO":
                        Fee = DailyFeeCalculator(7);
                        break;
                    case "EMPRENDEDOR":
                        Fee = DailyFeeCalculator(7.5);
                        break;
                    case "EMPRESARIO":
                        Fee = DailyFeeCalculator(8);
                        break;
                    case "FINANCIERO":
                        Fee = DailyFeeCalculator(9);
                        break;
                    case "ELITE":
                        Fee = DailyFeeCalculator(9.8);
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
                    Name = $"Abono a Utilidades {String.Format("{0:0.##}", profit)} ",
                    BalanceBefore = oldBalance,
                    CashOut = 0,
                    BalanceAfter = record.BalanceAvailable
                };
                _context.MovementsByBalance.Add(movement);
            }

            _cloudwatchLogs.InsertLogs("Cron", "cron", "Success");
            _context.SaveChanges();
            return Ok();
        }


        private double DailyFeeCalculator(double fee)
        {
            var montlyFee = fee / 100;
            return (montlyFee / 30);
        }
    }
}