using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpresariosConLiderazgo.Services
{
    public interface IDocumentService
    {
        byte[] GeneratePdfFromString();
    }
}
