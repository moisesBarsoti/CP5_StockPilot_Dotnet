using Microsoft.EntityFrameworkCore;
using StockPilot.Application.Interfaces.Repositories;
using StockPilot.Domain.Entities;
using StockPilot.Infrastructure.Data;

namespace StockPilot.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StockPilotDbContext _context;

    public ProductRepository(StockPilotDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(IEnumerable<Product> Items, int TotalItems)>
        GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Products.AsNoTracking();

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}