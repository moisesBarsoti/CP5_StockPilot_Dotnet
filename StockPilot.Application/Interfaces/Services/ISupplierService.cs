using StockPilot.Application.Common.Results;
using StockPilot.Application.DTOs.Supplier;

namespace StockPilot.Application.Interfaces.Services;

public interface ISupplierService
{
    Task<OperationResult<SupplierResponse>> CreateAsync(SupplierRequest request);

    Task<OperationResult<SupplierResponse>> GetByIdAsync(Guid id);
}