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
using EmpresariosConLiderazgo.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpresariosConLiderazgo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CronController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICloudwatchLogs _cloudwatchLogs;
        private readonly IMailService mailService;

        public CronController(ApplicationDbContext context, ICloudwatchLogs cloudwatchLogs, IMailService mailService)
        {
            _context = context;
            _cloudwatchLogs = cloudwatchLogs;
            this.mailService = mailService;
        }

        [HttpGet]
        public async Task<IActionResult> ExecuteCron()
        {
            var records = await _context.Balance.Where(x => x.StatusBalance == Utils.EnumStatusBalance.APROBADO)
                .ToListAsync();


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
                await _context.MovementsByBalance.AddAsync(movement);
            }

            await _cloudwatchLogs.InsertLogs("Cron", "cron", "Success");
            await _context.SaveChangesAsync();

            await SendNotification();
            return Ok();
        }


        private async Task SendNotification()
        {
            var listEmail = new List<string>()
            {
                "empresarios.riqueza@gmail.com",
                "m.logueo123@gmail.com"
            };

            var date = DateTime.UtcNow;
            foreach (var mail in listEmail)
            {
                var request = new MailRequest
                {
                    Subject = $"Intereses Aplicados {date} ",
                    Body = "Se aplicaron los intereses con exito",
                    ToEmail = mail.ToString()
                };
                await mailService.SendEmailAsync(request);
            }
        }


        private double DailyFeeCalculator(double fee)
        {
            var montlyFee = fee / 100;
            return (montlyFee / 30);
        }
    }
}