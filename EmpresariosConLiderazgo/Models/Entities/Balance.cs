using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpresariosConLiderazgo.Models.Entities
{
    public class Balance : BaseClass
    {
        public string UserApp { get; set; }
        public string Product { get; set; }
        public float BalanceAvailable { get; set; }
        public DateTime LastMovement { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime EndlDate { get; set; }


    }
}
