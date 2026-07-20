using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InvoiceBackend
{
    public class GenerateInvoice
    {
        private readonly ILogger<GenerateInvoice> _logger;
        private readonly IInvoiceService _invoiceService;

        // IInvoiceRepository has been removed from here!
        public GenerateInvoice(
            ILogger<GenerateInvoice> logger,
            IInvoiceService invoiceService)
        {
            _logger = logger;
            _invoiceService = invoiceService;
        }

        [Function("GenerateInvoice")]
        public async Task<IActionResult> HandleInvoiceGeneration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "invoice/generate")] HttpRequest req)
        {
            _logger.LogInformation("Processing invoice storage and PDF generation pipeline.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                // 1. Safe deserialization using nullable type
                InvoiceModel? invoice = JsonConvert.DeserializeObject<InvoiceModel>(requestBody);

                // 2. Clear validation checking if request is completely empty or missing required data
                if (invoice == null || string.IsNullOrWhiteSpace(invoice.CustomerName))
                {
                    return new BadRequestObjectResult("Invalid invoice data. Customer Name is required.");
                }

                // 3. The Service now handles BOTH saving to the database AND generating the PDF
                byte[] pdfBytes = await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

                _logger.LogInformation("Invoice pipeline completed successfully.");

                return new FileContentResult(pdfBytes, "application/pdf")
                {
                    FileDownloadName = $"Invoice_{invoice.CustomerName}.pdf"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error handled inside execution pipeline.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}