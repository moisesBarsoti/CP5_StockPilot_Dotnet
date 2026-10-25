using Microsoft.EntityFrameworkCore;
using StockPilot.Application.Interfaces.Repositories;
using StockPilot.Domain.Entities;
using StockPilot.Infrastructure.Data;

namespace StockPilot.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly StockPilotDbContext _context;

    public SupplierRepository(StockPilotDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(Guid id)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddAsync(Supplier supplier)
    {
        await _context.Suppliers.AddAsync(supplier);
    }

    public void Update(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
    }

    public void Delete(Supplier supplier)
    {
        _context.Suppliers.Remove(supplier);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Suppliers
            .AnyAsync(s => s.Id == id);
    }
}