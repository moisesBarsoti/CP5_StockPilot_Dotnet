using StockPilot.Domain.Entities;

namespace StockPilot.Application.Interfaces.Repositories;

public interface ISupplierRepository
{
    Task<IEnumerable<Supplier>> GetAllAsync();

    Task<Supplier?> GetByIdAsync(Guid id);

    Task AddAsync(Supplier supplier);

    void Update(Supplier supplier);

    void Delete(Supplier supplier);

    Task<bool> ExistsAsync(Guid id);
}