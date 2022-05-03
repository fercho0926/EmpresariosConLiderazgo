using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpresariosConLiderazgo.Models.Entities
{
    public class Movements : BaseClass
    {
        public string IdBalanceProduct { get; set; }
        public DateTime DateMovement { get; set; }
        public string BalanceBefore { get; set; }
        public string BalanceAfter { get; set; }
    }
}
