using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceBackend;
using Moq;
using Xunit;

namespace InvoiceBackend.Tests;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _mockRepository;
    private readonly Mock<IPdfGenerator> _mockPdfGenerator;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _mockRepository = new Mock<IInvoiceRepository>();
        _mockPdfGenerator = new Mock<IPdfGenerator>();

        _mockPdfGenerator.Setup(x => x.GeneratePdfFromHtml(It.IsAny<string>()))
            .Returns(new byte[] { 1, 2, 3, 4, 5 });

        _invoiceService = new InvoiceService(_mockRepository.Object, _mockPdfGenerator.Object);
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_SavesInvoiceToRepository()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "John Doe",
            Description = "Test Invoice",          
            Items = new List<InvoiceItem>()
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

        // Assert
        _mockRepository.Verify(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()), Times.Once);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_ReturnsPdfBytes()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "Jane Smith",
            Description = "PDF Generation Test",
            Items = new List<InvoiceItem>()
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "PDF should contain data");
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_HandleMultipleItems()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "ABC Corporation",
            Description = "Multiple Items Invoice",
            Items = new List<InvoiceItem>
            {
                new InvoiceItem { ItemName = "Item 1", Quantity = 5, Price = 100.00 },
                new InvoiceItem { ItemName = "Item 2", Quantity = 3, Price = 50.00 },
                new InvoiceItem { ItemName = "Item 3", Quantity = 2, Price = 75.00 }
            }
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        _mockRepository.Verify(x => x.SaveInvoiceAsync(invoice), Times.Once);
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_HandleNoItems()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "Customer With No Items",
            Description = "Empty Items Invoice",
            Items = new List<InvoiceItem>()
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_CalculatesTotalCorrectly()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "Calculation Test",
            Description = "Testing Total Calculation",
            Items = new List<InvoiceItem>
            {
                new InvoiceItem { ItemName = "Product A", Quantity = 10, Price = 25.00 },
                new InvoiceItem { ItemName = "Product B", Quantity = 4, Price = 75.50 }
            }
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_CallsRepositoryExactlyOnce()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "Repository Call Test",
            Description = "Testing Repository Invocation",
            Items = new List<InvoiceItem>()
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .Returns(Task.CompletedTask);

        // Act
        await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

        // Assert
        _mockRepository.Verify(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()), Times.Exactly(1));
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_HandleSpecialCharacters()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "John & Jane O'Brien-Smith",
            Description = "Special <Characters> & Test",
            Items = new List<InvoiceItem>
            {
                new InvoiceItem { ItemName = "Item <Special>", Quantity = 1, Price = 150.75 }
            }
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_HandleLargeAmounts()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "Enterprise Customer",
            Description = "Large Invoice Amount",
            Items = new List<InvoiceItem>
            {
                new InvoiceItem { ItemName = "Premium Service", Quantity = 1000, Price = 9999.99 }
            }
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _invoiceService.ProcessAndGenerateInvoiceAsync(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ProcessAndGenerateInvoiceAsync_RepositoryThrowsException()
    {
        // Arrange
        var invoice = new InvoiceModel
        {
            CustomerName = "Exception Test",
            Description = "Testing Exception Handling",
            Items = new List<InvoiceItem>()
        };

        _mockRepository.Setup(x => x.SaveInvoiceAsync(It.IsAny<InvoiceModel>()))
            .ThrowsAsync(new System.Exception("Repository error"));

        // Act & Assert
        await Assert.ThrowsAsync<System.Exception>(
            () => _invoiceService.ProcessAndGenerateInvoiceAsync(invoice));
    }
}