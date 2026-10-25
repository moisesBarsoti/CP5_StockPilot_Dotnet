using Microsoft.EntityFrameworkCore;
using StockPilot.Application.Interfaces.Repositories;
using StockPilot.Domain.Entities;
using StockPilot.Infrastructure.Data;
using StockPilot.Application.Common.Results;
using StockPilot.Domain.Exceptions;

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

    
    public async Task<Supplier> GetByIdAsync(Guid id)
    {
        var supplier = await _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        return supplier ?? throw new SupplierNotFoundException(id);
    }
    
    
    public async Task<OperationResult<bool>> CheckEmailAvailabilityAsync(string email)
    {
        var exists = await _context.Suppliers.AsNoTracking().AnyAsync(s => s.Email == email);

        if (exists)
        {
            return OperationResult<bool>.Failure(
                "SUPPLIER_EMAIL_ALREADY_EXISTS",
                "Já existe um fornecedor cadastrado com este e-mail."
            );
        }

        return OperationResult<bool>.Success(true);
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