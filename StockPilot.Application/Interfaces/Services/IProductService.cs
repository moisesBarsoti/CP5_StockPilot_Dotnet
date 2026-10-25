using StockPilot.Application.Common.Pagination;
using StockPilot.Application.Common.Results;
using StockPilot.Application.DTOs.Product;

namespace StockPilot.Application.Interfaces.Services;

public interface IProductService
{
    Task<OperationResult<ProductResponse>> CreateAsync(
        ProductRequest request);

    Task<OperationResult<ProductResponse>> GetByIdAsync(
        Guid id);

    Task<OperationResult<PageResult<ProductResponse>>> GetPagedAsync(
        int page, int pageSize);
}