using EmpresariosConLiderazgo.Utils;
using System.ComponentModel.DataAnnotations;

namespace EmpresariosConLiderazgo.Models.Entities
{
    public class MovementsByBalance : BaseClass
    {
        [Display(Name = "Fecha movimiento")] public DateTime DateMovement { get; set; }
        [Display(Name = "Saldo anterior")] public decimal BalanceBefore { get; set; }
        [Display(Name = "Valor retiro")] public decimal CashOut { get; set; }
        [Display(Name = "Saldo despues")] public decimal BalanceAfter { get; set; }
        [Display(Name = "Estado")] public EnumStatus status { get; set; }
        public int BalanceId { get; set; }
    }
}