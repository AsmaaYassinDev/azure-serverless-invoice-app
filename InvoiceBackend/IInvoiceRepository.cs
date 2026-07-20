using System.Threading.Tasks;

namespace InvoiceBackend
{
    public interface IInvoiceRepository
    {
        Task SaveInvoiceAsync(InvoiceModel invoice);
        Task<InvoiceModel> GetInvoiceByIdAsync(string id);
    }
}