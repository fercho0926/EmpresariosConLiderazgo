using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpresariosConLiderazgo.Models;


using iTextSharp.text.pdf;

namespace EmpresariosConLiderazgo.Utils
{
    public class GeneratePDF
    {

        public GeneratePDF()
        {

        }
        public string GenerateInvestorDocument(User_contracts contractInfo)
        {
            string fullName = string.Concat(contractInfo.Approved, " ", contractInfo.Product);

            string filePath = @"Contratos\";

            string fileNameExisting = @"TemplateContract_KaizenForce.pdf";
            string fileNameNew = @"KaizenForce_" + fullName.Replace(" ", "").Trim() + ".pdf";

            string fullNewPath = filePath + fileNameNew;
            string fullExistingPath = filePath + fileNameExisting;

            using (var existingFileStream = new FileStream(fullExistingPath, FileMode.Open))

            using (var newFileStream = new FileStream(fullNewPath, FileMode.Create))
            {
                // Open existing PDF
                var pdfReader = new PdfReader(existingFileStream);

                // PdfStamper, which will create
                var stamper = new PdfStamper(pdfReader, newFileStream);

                AcroFields fields = stamper.AcroFields;
                fields.SetField("FullName", fullName);
                fields.SetField("DocumentNumber", contractInfo.Product);
                fields.SetField("Date", DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-ES")));

                // "Flatten" the form so it wont be editable/usable anymore
                stamper.FormFlattening = true;

                stamper.Close();
                pdfReader.Close();

                return fileNameNew;
            }
        }
    }
}


