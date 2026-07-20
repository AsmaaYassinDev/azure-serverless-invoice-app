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
            if (string.IsNullOrEmpty(invoice.Id))
            {
                invoice.Id = Guid.NewGuid().ToString();
            }

            await _container.CreateItemAsync(invoice, new PartitionKey(invoice.Id));
        }
        public async Task<InvoiceModel> GetInvoiceByIdAsync(string id)
        {
            try
            {
                // _container is your Cosmos DB container variable created in the repository constructor
                var response = await _container.ReadItemAsync<InvoiceModel>(
                    id: id,
                    partitionKey: new PartitionKey(id)
                );

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // If it doesn't exist, gracefully return null instead of crashing
                return null;
            }
        }
    }
}