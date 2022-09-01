using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpresariosConLiderazgo.Models.Entities;

namespace EmpresariosConLiderazgo.Models
{
    public class Balance_ReferenceByuser
    {
        public Balance Balance { get; set; }
        public ReferedByUser ReferedByUser { get; set; }
    }
}