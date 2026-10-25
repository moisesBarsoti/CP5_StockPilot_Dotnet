using StockPilot.Domain.Entities;

namespace StockPilot.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);

    Task<(IEnumerable<Product> Items, int TotalItems)>
        GetPagedAsync(int page, int pageSize);

    Task AddAsync(Product product);

    void Update(Product product);

    void Delete(Product product);

    Task<bool> ExistsAsync(Guid id);
}