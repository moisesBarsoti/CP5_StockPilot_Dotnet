
using Microsoft.AspNetCore.Mvc;
using StockPilot.Application.DTOs.Product;
using StockPilot.Application.Interfaces.Services;

namespace StockPilot.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] ProductRequest request)
    {
        var result = await _productService.CreateAsync(request);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "SUPPLIER_NOT_FOUND")
                return NotFound(result);

            return BadRequest(result);
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
        var result = await _productService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(new
        {
            data = result.Data,
            links = new
            {
                self = Url.ActionLink(
                    nameof(GetById),
                    values: new { id }
                ),
                collection = Url.ActionLink(
                    nameof(GetPaged)
                )
            }
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _productService
            .GetPagedAsync(page, pageSize);

        if (!result.IsSuccess)
            return BadRequest(result);

        var pagination = result.Data!;

        return Ok(new
        {
            data = pagination,
            links = new
            {
                self = Url.ActionLink(
                    nameof(GetPaged),
                    values: new { page, pageSize }
                ),

                next = page < pagination.TotalPages
                    ? Url.ActionLink(
                        nameof(GetPaged),
                        values: new { page = page + 1, pageSize }
                    )
                    : null,

                previous = page > 1
                    ? Url.ActionLink(
                        nameof(GetPaged),
                        values: new { page = page - 1, pageSize }
                    )
                    : null
            }
        });
    }
}
