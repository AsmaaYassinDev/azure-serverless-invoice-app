
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Xunit;
using FluentAssertions;
using Moq;
using InvoiceBackend;

namespace InvoiceBackend.IntegrationTests
{
    public class CosmosDbIntegrationTests
    {
        [Fact]
        public async Task SaveInvoice_ToRealDatabase_ShouldPersistCorrectly()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            string testConnectionString = config.GetConnectionString("CosmosDb");

            var cosmosClient = new CosmosClient(testConnectionString);
            var realRepository = new InvoiceRepository(cosmosClient);

            var mockPdfGenerator = new Mock<IPdfGenerator>();
            mockPdfGenerator.Setup(x => x.GeneratePdfFromHtml(It.IsAny<string>()))
                            .Returns(new byte[] { 1, 2, 3 });

            var invoiceService = new InvoiceService(realRepository, mockPdfGenerator.Object);

            var newInvoice = new InvoiceModel
            {
                Id = "Test"+ Guid.NewGuid().ToString(),
                CustomerName = "Integration Test Corp",
                Description = "Testing the live database connection",

                // 1. Amount is removed! It will calculate automatically based on the items below.

                // 2. Add an item so the calculated total becomes exactly 1500.00
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { ItemName = "Software License", Quantity = 1, Price = 1500.00 }
                }
            };

            // Act
            await invoiceService.ProcessAndGenerateInvoiceAsync(newInvoice);

            // Assert
            var savedInvoice = await realRepository.GetInvoiceByIdAsync(newInvoice.Id);

            savedInvoice.Should().NotBeNull("because it should have been saved to Cosmos DB");
            savedInvoice.CustomerName.Should().Be("Integration Test Corp");

            // This assertion will now pass because the Items list calculates to 1500!
            savedInvoice.Amount.Should().Be(1500.00);
        }
    }
}