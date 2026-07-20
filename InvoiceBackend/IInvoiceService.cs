using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceBackend
{
    public interface IInvoiceService
    {
        Task<byte[]> ProcessAndGenerateInvoiceAsync(InvoiceModel invoice);
    }
}
