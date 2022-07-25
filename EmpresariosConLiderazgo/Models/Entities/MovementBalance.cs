using EmpresariosConLiderazgo.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpresariosConLiderazgo.Models.Entities
{
    public class MovementBalance
    {

        public int BalanceId { get; set; }
        public int MovementId { get; set; }


        [Display(Name = "Usuario")] public string? UserApp { get; set; }
        [Display(Name = "Producto")] public string? Product { get; set; }

        [Display(Name = "Saldo disponible")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal BalanceAvailable { get; set; }

        [Display(Name = "Saldo Base")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal BaseBalanceAvailable { get; set; }

        [Display(Name = "Rentabilidad")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal Profit { get; set; }
 


        //MOVEMENTS
 

        [Display(Name = "Fecha Solicitud Retiro")] public DateTime DateMovement { get; set; }
        [Display(Name = "Valor a retirar")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal CashOut { get; set; }
        [Display(Name = "Saldo despues")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal BalanceAfter { get; set; }
        [Display(Name = "Estado")] public EnumStatus Status { get; set; }

    }

}
