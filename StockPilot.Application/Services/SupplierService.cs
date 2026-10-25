
using StockPilot.Application.Common.Results;
using StockPilot.Application.DTOs.Supplier;
using StockPilot.Application.Interfaces.External;
using StockPilot.Application.Interfaces.Repositories;
using StockPilot.Application.Interfaces.Services;
using SupplierEntity = StockPilot.Domain.Entities.Supplier;

namespace StockPilot.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IZipCodeService _zipCodeService;

    public SupplierService(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        IZipCodeService zipCodeService)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _zipCodeService = zipCodeService;
    }

    public async Task<OperationResult<SupplierResponse>> CreateAsync(
        SupplierRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var availability = await _supplierRepository
            .CheckEmailAvailabilityAsync(email);

        if (!availability.IsSuccess)
        {
            return OperationResult<SupplierResponse>.Failure(
                availability.ErrorCode ?? "SUPPLIER_EMAIL_ALREADY_EXISTS",
                availability.ErrorMessage ?? "E-mail já cadastrado."
            );
        }

        var addressResult = await _zipCodeService
            .FindAsync(request.ZipCode);

        if (!addressResult.IsSuccess || addressResult.Data is null)
        {
            return OperationResult<SupplierResponse>.Failure(
                addressResult.ErrorCode ?? "ZIP_CODE_ERROR",
                addressResult.ErrorMessage ?? "Erro ao consultar CEP."
            );
        }

        var supplier = new SupplierEntity
        {
            Name = request.Name.Trim(),
            Email = email,
            ZipCode = request.ZipCode,
            City = addressResult.Data.City,
            State = addressResult.Data.State
        };

        await _supplierRepository.AddAsync(supplier);
        await _unitOfWork.SaveChangesAsync();

        return OperationResult<SupplierResponse>.Success(
            MapToResponse(supplier)
        );
    }

    public async Task<OperationResult<SupplierResponse>> GetByIdAsync(
        Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);

        return OperationResult<SupplierResponse>.Success(
            MapToResponse(supplier)
        );
    }

    private static SupplierResponse MapToResponse(
        SupplierEntity supplier)
    {
        return new SupplierResponse
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Email = supplier.Email,
            ZipCode = supplier.ZipCode,
            City = supplier.City,
            State = supplier.State,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt
        };
    }
}
