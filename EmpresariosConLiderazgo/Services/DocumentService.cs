using DinkToPdf;
using DinkToPdf.Contracts;

namespace EmpresariosConLiderazgo.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IConverter _converter;


        public DocumentService(
            IConverter converter)

        {
            _converter = converter;
        }

        public byte[] GeneratePdfFromString()
        {
            var htmlContent = $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <style>
                p{{
                    width: 95%;
                    font-family: Arial;
                    font-size: 1.2em;
                }}
                </style>
            </head>
            <body>
                <h1>Contrato : Empresarios Con Liderazgo</h1>
                <h4>Medellin {DateTime.Now}</h4>
                <p><strong>CONVENIO DE INVERSIÓN</strong> entre <strong>EMPRESARIOS CON LIDERAZGO con NIT 1128469767-1,</strong> gestor de capitales de la ciudad de Medellín, Antioquia. 
                    Ubicados en edificio REGUS poblado en la dirección Cra 43 N° 11ª 44 oficina 206. 
                    Conste por el presente documento, el Convenio de Inversión con inversionista <strong>XXXXXXXXXXXXXXXXX, CC XXXXXXXXXX </strong>. Mayor de edad,
                    quien de ahora en adelante firma convenio de administración de capital por un total de <strong> XXXXXXXXX, (XXXXXXXXXX)</strong>, 
                    del cual recibirá una utilidad del <strong>X%</strong>, cada mes, este convenio es mutuo acuerdo, e intransferible a segundas o terceras personas;
                    ya que es de única persona.<br><strong> Clausula 1</strong> Este convenio y ejecución se realiza a cambio de producción de activos en mercados bursátiles tales como,
                    inversiones en trading, compra de acciones, entre otras actividades que respaldan nuestra inversión. 
                    <br><strong>Clausula 2</strong> Este convenio tendrá una duración de <strong>X</strong> meses iniciando el <strong>xx de xxx 202x</strong> y finalización del contrato <strong>xx de xx 202x.</strong>
                    <br><strong>Clausula 3</strong> Los pagos de los porcentajes se realizarán en los <strong>primeros 5 días de cada mes</strong>, en esos primeros 5 días recibirá vía WhatsApp, o correo electrónico una confirmación de pago,
                    en caso de no recibir la confirmación, avisar a la compañía al número de WhatsApp<strong> +57300-333-0745</strong>, o al correo electrónico <strong>empresarios.admon@gmail.com.</strong>
                    <br><strong>Clausula 4</strong> El pago del primer mes se liquida sobre los días hábiles de inversión, es decir la diferencia entre el día que se hace el pago y los días que pasaron para el fin de mes.
                    <br><strong>Clausula 5</strong> Para cancelación de convenio y solicitud de retiro del capital, lo debe hacer con 2 meses de anticipación, en caso de retirar antes de la finalización del convenio,
                    el inversionista asumirá una penalidad del 20% sobre la inversión realizada.
                    <br><br>
                    <strong>TERMINOS Y CONDICIONES</strong> Nuestros financieros tienen la oportunidad de invertir en diferentes tipos de contratos, largo, mediano, y corto plazo, son autónomos y responsables de sus propias decisiones,
                    sus datos serán tratados con responsabilidad, se les informara de eventos e inversiones, foros que realizamos desde nuestra compañía,
                    tiene derecho a participar y estar enterado(a) de cómo se mueven los mercados financieros, puede abrir los convenios de inversión que desee,
                    puede ingresar a nuestra página web,<strong> www.empresariosconliderazgo.org</strong>.
                    <br>Es muy importante que tenga en cuenta a mayor inversión, mayores ganancias.
                     <br><br>
                    <strong>Aunque el riesgo es bajo, no lo exime de los riesgos que conlleva operar en los mercados bursátiles.</strong>
                      <br><br>
                      <br><br>___________________________________________________
                    <br><strong>Inversionista :</strong>
                    <br><strong> Documento: </strong>

                     <br><br>___________________________________________________
                    <br><strong>Representante Legal :</strong> Uber Meneses Vásquez 
                    <br> <strong>Nit:</strong> 1128469767 -1 
                    <br> <strong>Empresarios Con Liderazgo</strong></p>
            </body>
            </html>
            ";

            return GeneratePdf(htmlContent);
        }


        private byte[] GeneratePdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 18, Bottom = 18 },
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Pagina [page] de [toPage]", Line = true },
                FooterSettings =
                    { FontSize = 8, Center = "Empresarios Con Liderazgo, NIT 1128469767 -1 ", Line = true },
            };

            var htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }
    }
}