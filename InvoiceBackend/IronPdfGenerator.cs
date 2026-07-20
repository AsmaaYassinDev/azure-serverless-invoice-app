using IronPdf;

namespace InvoiceBackend
{
    public class IronPdfGenerator : IPdfGenerator
    {
        public byte[] GeneratePdfFromHtml(string htmlContent)
        {
            var renderer = new ChromePdfRenderer();
            var pdf = renderer.RenderHtmlAsPdf(htmlContent);
            return pdf.BinaryData;
        }
    }
}