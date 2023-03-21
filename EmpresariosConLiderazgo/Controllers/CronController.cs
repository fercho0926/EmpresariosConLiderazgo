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
                if (DateTime.Now < record.EndlDate)
                {
                    decimal Fee = 0;
                    switch (record.Product)
                    {
                        case "INICIO":
                            Fee = 0.04m;
                            break;
                        case "PLUS":
                            Fee = 0.05m;
                            break;
                        case "STAR":
                            Fee = 0.065m;
                            break;
                        case "ASOCIADO":
                            Fee = 0.07m;
                            break;
                        case "EMPRENDEDOR":
                            Fee = 0.075m;
                            break;
                        case "EMPRESARIO":
                            Fee = 0.08m;
                            break;
                        case "FINANCIERO":
                            Fee = 0.09m;
                            break;
                        case "ELITE":
                            Fee = 0.098m;
                            break;
                    }

                    var daysPerThisMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    var profit = DailyFee(record.BaseBalanceAvailable, daysPerThisMonth, Fee);
                    var oldBalance = record.BalanceAvailable;
                    record.BalanceAvailable += profit;
                    record.Profit += profit;

                    var movement = new MovementsByBalance
                    {
                        BalanceId = record.Id,
                        DateMovement = DateTime.Now,
                        Name = $"Abono a Utilidades {profit:0.##} ",
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
            foreach (var request in listEmail.Select(mail => new MailRequest
                     {
                         Subject = $"Intereses Aplicados {date} ",
                         Body = "Se aplicaron los intereses con exito",
                         ToEmail = mail.ToString()
                     }))
            {
                await mailService.SendEmailAsync(request);
            }
        }


        private static decimal DailyFee(decimal balance, int monthDays, decimal monthlyFee)
        {
            var dailyFee = monthlyFee / monthDays;
            return balance * dailyFee;
        }
    }
}