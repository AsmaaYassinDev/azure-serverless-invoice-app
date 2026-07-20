using System;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceBackend
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPdfGenerator _pdfGenerator;

        public InvoiceService(IInvoiceRepository invoiceRepository, IPdfGenerator pdfGenerator)
        {
            _invoiceRepository = invoiceRepository;
            _pdfGenerator = pdfGenerator;
        }

        // 2. This method now coordinates the business rules: Save first, then Generate.
        public async Task<byte[]> ProcessAndGenerateInvoiceAsync(InvoiceModel invoice)
        {
            // Step A: Save the invoice to Cosmos DB
            await _invoiceRepository.SaveInvoiceAsync(invoice);

            // Step B: Generate the HTML string
            string htmlContent = BuildHtmlTemplate(invoice);

            // Step C: Generate the PDF using the injected abstraction
            byte[] pdfBytes = _pdfGenerator.GeneratePdfFromHtml(htmlContent);

            return pdfBytes;
        }

        private string BuildHtmlTemplate(InvoiceModel invoice)
        {
            var itemsTableRows = new StringBuilder();
            double calculatedTotal = 0;

            if (invoice.Items != null)
            {
                foreach (var item in invoice.Items)
                {
                    double itemTotal = item.Quantity * item.Price;
                    calculatedTotal += itemTotal;

                    itemsTableRows.Append($@"
                    <tr>
                        <td>{item.ItemName}</td>
                        <td>{item.Quantity}</td>
                        <td>${item.Price:N2}</td>
                        <td>${itemTotal:N2}</td>
                    </tr>");
                }
            }

            if (invoice.Items == null || invoice.Items.Count == 0)
            {
                calculatedTotal = invoice.Amount;
            }

            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 30px; color: #333; }}
                    .header {{ text-align: center; border-bottom: 2px solid #333; padding-bottom: 10px; }}
                    .details {{ margin-top: 20px; font-size: 14px; margin-bottom: 20px; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                    th {{ background-color: #f2f2f2; }}
                    .total {{ text-align: right; margin-top: 30px; font-size: 18px; font-weight: bold; color: #0078d4; }}
                </style>
            </head>
            <body>
                <div class='header'><h1>INVOICE</h1></div>
                <div class='details'>
                    <p><strong>Customer Name:</strong> {invoice.CustomerName}</p>
                    <p><strong>Description:</strong> {invoice.Description}</p>
                    <p><strong>Date:</strong> {DateTime.UtcNow.ToShortDateString()}</p>
                </div>
                <table>
                    <thead>
                        <tr>
                            <th>Item Name</th>
                            <th>Quantity</th>
                            <th>Unit Price</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>{itemsTableRows}</tbody>
                </table>
                <div class='total'>Grand Total: ${calculatedTotal:N2}</div>
            </body>
            </html>";
        }
    }
}