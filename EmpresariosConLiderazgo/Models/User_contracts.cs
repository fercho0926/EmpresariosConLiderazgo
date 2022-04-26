using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpresariosConLiderazgo.Models
{
    public class User_contracts : BaseClass
    {
        public string UserContract { get; set; }
        public int Version { get; set; }
        public string Product { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string S3Route { get; set; }

        public bool Approved { get; set; }


    }
}
