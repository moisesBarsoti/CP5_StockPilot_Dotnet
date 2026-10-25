using Microsoft.AspNetCore.Mvc;
using StockPilot.Application.DTOs.Supplier;
using StockPilot.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace StockPilot.API.Controllers;

[ApiController]
[Authorize]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        [FromBody] SupplierRequest request)
    {
        var result = await _supplierService.CreateAsync(request);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "EXTERNAL_SERVICE_UNAVAILABLE" =>
                    StatusCode(503, result),

                "INVALID_EXTERNAL_RESPONSE" =>
                    StatusCode(502, result),
                
                "SUPPLIER_EMAIL_ALREADY_EXISTS" =>
                    Conflict(result),
                
                _ => BadRequest(result)
            };
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result.Data
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _supplierService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Data);
    }
}