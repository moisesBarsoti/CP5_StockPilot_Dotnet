
using StockPilot.Application.Common.Pagination;
using StockPilot.Application.Common.Results;
using StockPilot.Application.DTOs.Product;
using StockPilot.Application.Interfaces.Repositories;
using StockPilot.Application.Interfaces.Services;
using ProductEntity = StockPilot.Domain.Entities.Product;

namespace StockPilot.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<ProductResponse>> CreateAsync(
        ProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            request.Price <= 0 ||
            request.StockQuantity < 0 ||
            request.SupplierId == Guid.Empty)
        {
            return OperationResult<ProductResponse>.Failure(
                "INVALID_PRODUCT",
                "Os dados do produto são inválidos."
            );
        }

        var supplierExists = await _supplierRepository
            .ExistsAsync(request.SupplierId);

        if (!supplierExists)
        {
            return OperationResult<ProductResponse>.Failure(
                "SUPPLIER_NOT_FOUND",
                "Fornecedor não encontrado."
            );
        }

        var product = new ProductEntity
        {
            Name = request.Name.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            SupplierId = request.SupplierId
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return OperationResult<ProductResponse>.Success(
            MapToResponse(product)
        );
    }

    public async Task<OperationResult<ProductResponse>> GetByIdAsync(
        Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return OperationResult<ProductResponse>.Failure(
                "PRODUCT_NOT_FOUND",
                "Produto não encontrado."
            );
        }

        return OperationResult<ProductResponse>.Success(
            MapToResponse(product)
        );
    }

    public async Task<OperationResult<PageResult<ProductResponse>>>
        GetPagedAsync(int page, int pageSize)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return OperationResult<PageResult<ProductResponse>>.Failure(
                "INVALID_PAGINATION",
                "Página ou tamanho da página inválido."
            );
        }

        var (items, totalItems) = await _productRepository
            .GetPagedAsync(page, pageSize);

        var products = items.Select(MapToResponse).ToList();

        var result = new PageResult<ProductResponse>(
            products,
            page,
            pageSize,
            totalItems
        );

        return OperationResult<PageResult<ProductResponse>>.Success(
            result
        );
    }

    private static ProductResponse MapToResponse(
        ProductEntity product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            SupplierId = product.SupplierId,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
