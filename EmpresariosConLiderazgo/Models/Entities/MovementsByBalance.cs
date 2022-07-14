using EmpresariosConLiderazgo.Utils;
using System.ComponentModel.DataAnnotations;

namespace EmpresariosConLiderazgo.Models.Entities
{
    public class MovementsByBalance : BaseClass
    {
        [Display(Name = "Fecha movimiento")] public DateTime DateMovement { get; set; }
        [Display(Name = "Saldo anterior")]
        [DisplayFormat(DataFormatString = "{0:N0} ")] 
        public decimal BalanceBefore { get; set; }
        [Display(Name = "Valor retiro")]
        [DisplayFormat(DataFormatString = "{0:N0} ")] 
        public decimal CashOut { get; set; }
        [Display(Name = "Saldo despues")]
        [DisplayFormat(DataFormatString = "{0:N0} ")] 
        public decimal BalanceAfter { get; set; }
        [Display(Name = "Estado")] public EnumStatus status { get; set; }
        public int BalanceId { get; set; }
    }
}