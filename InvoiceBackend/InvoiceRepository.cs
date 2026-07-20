using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace InvoiceBackend
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly Container _container;

        public InvoiceRepository(CosmosClient cosmosClient)
        {
            // Must match the exact strings from your emulator setup
            string databaseId = "InvoiceDB";
            string containerId = "Invoices";

            _container = cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task SaveInvoiceAsync(InvoiceModel invoice)
        {
            // Ensure your InvoiceModel has a lowercase "id" property populated
            if (string.IsNullOrEmpty(invoice.id))
            {
                invoice.id = Guid.NewGuid().ToString();
            }

            await _container.CreateItemAsync(invoice, new PartitionKey(invoice.id));
        }
    }
}