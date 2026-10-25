
using Moq;
using StockPilot.Application.DTOs.Product;
using StockPilot.Application.Interfaces.Repositories;
using StockPilot.Application.Services;

namespace StockPilot.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ISupplierRepository> _supplierRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepository = new Mock<IProductRepository>();
        _supplierRepository = new Mock<ISupplierRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _productService = new ProductService(
            _productRepository.Object,
            _supplierRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenPriceIsNegative()
    {
        var request = new ProductRequest
        {
            Name = "Mouse Gamer",
            Price = -10,
            StockQuantity = 5,
            SupplierId = Guid.NewGuid()
        };

        var result = await _productService.CreateAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("INVALID_PRODUCT", result.ErrorCode);

        _productRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<StockPilot.Domain.Entities.Product>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenSupplierDoesNotExist()
    {
        var request = new ProductRequest
        {
            Name = "Teclado Gamer",
            Price = 150,
            StockQuantity = 10,
            SupplierId = Guid.NewGuid()
        };

        _supplierRepository
            .Setup(repository => repository.ExistsAsync(
                request.SupplierId))
            .ReturnsAsync(false);

        var result = await _productService.CreateAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal("SUPPLIER_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFail_WhenPageIsInvalid()
    {
        var result = await _productService.GetPagedAsync(0, 10);

        Assert.False(result.IsSuccess);
        Assert.Equal("INVALID_PAGINATION", result.ErrorCode);
    }
}
