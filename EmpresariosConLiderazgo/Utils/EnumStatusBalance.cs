using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpresariosConLiderazgo.Utils
{
    public enum EnumStatusBalance
    {
        [Display(Name = "PENDIENTE DE APROBACION")]
        PENDIENTE,
        APROBADO,
        RECHAZADO,
        FINALIZADO,
        POR_RETIRAR,
        SOLICITUD_RETIRO
    }
}