using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpresariosConLiderazgo.Utils;

namespace EmpresariosConLiderazgo.Models.Entities
{
    public class Balance : BaseClass
    {
        [Display(Name = "Usuario")] public string UserApp { get; set; }
        [Display(Name = "Producto")] public string Product { get; set; }

        [Display(Name = "Saldo disponible")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal BalanceAvailable { get; set; }

        [Display(Name = "Saldo Base")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal BaseBalanceAvailable { get; set; }

        [Display(Name = "Rentabilidad")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal Profit { get; set; }

        [Display(Name = "Modeda")] public EnumCurrencies Currency { get; set; }

        [Display(Name = "Retirar")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal CashOut { get; set; }

        [Display(Name = "Ultimo movimiento")] public DateTime LastMovement { get; set; }
        [Display(Name = "Fecha inicio")] public DateTime InitialDate { get; set; }
        [Display(Name = "Fecha fin")] public DateTime EndlDate { get; set; }

        [Display(Name = "Estado")] public EnumStatusBalance StatusBalance { get; set; }

        public bool Contract { get; set; } // 0 sin contrato 1 co contrato


        public List<MovementsByBalance> MovementsByBalance { get; set; }
    }
}