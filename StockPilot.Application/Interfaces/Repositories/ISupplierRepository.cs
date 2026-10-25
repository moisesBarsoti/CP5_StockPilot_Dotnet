using StockPilot.Domain.Entities;
using StockPilot.Application.Common.Results;

namespace StockPilot.Application.Interfaces.Repositories;

public interface ISupplierRepository
{
    Task<IEnumerable<Supplier>> GetAllAsync();

    Task<Supplier> GetByIdAsync(Guid id);

    Task<OperationResult<bool>> CheckEmailAvailabilityAsync(string email);

    Task AddAsync(Supplier supplier);

    void Update(Supplier supplier);

    void Delete(Supplier supplier);

    Task<bool> ExistsAsync(Guid id);
}