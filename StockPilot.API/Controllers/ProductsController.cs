
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPilot.Application.DTOs.Product;
using StockPilot.Application.Interfaces.Services;

namespace StockPilot.API.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Cadastra um novo produto no estoque.
    /// </summary>
    /// <remarks>
    /// Cria um produto com nome, preço, quantidade em estoque
    /// e identificação do fornecedor.
    ///
    /// O fornecedor informado deve existir no banco de dados.
    ///
    /// Operação permitida somente para usuários com perfil ADMIN.
    /// </remarks>
    /// <param name="request">
    /// Informações necessárias para cadastrar o produto.
    /// </param>
    /// <response code="201">
    /// Produto cadastrado com sucesso.
    /// </response>
    /// <response code="400">
    /// Dados do produto inválidos.
    /// </response>
    /// <response code="401">
    /// Usuário não autenticado.
    /// </response>
    /// <response code="403">
    /// Usuário sem permissão para cadastrar produtos.
    /// </response>
    /// <response code="404">
    /// Fornecedor informado não encontrado.
    /// </response>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Consulta um produto pelo identificador.
    /// </summary>
    /// <remarks>
    /// Retorna as informações de um produto cadastrado no Oracle.
    ///
    /// A resposta inclui links HATEOAS para consultar o
    /// próprio produto e acessar a coleção de produtos.
    ///
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">
    /// Identificador único do produto (GUID).
    /// </param>
    /// <response code="200">
    /// Produto encontrado e retornado com links HATEOAS.
    /// </response>
    /// <response code="401">
    /// Usuário não autenticado.
    /// </response>
    /// <response code="404">
    /// Produto não encontrado.
    /// </response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Lista os produtos com paginação.
    /// </summary>
    /// <remarks>
    /// Consulta os produtos armazenados no Oracle Database.
    ///
    /// Retorna os registros da página solicitada, a quantidade
    /// total de produtos, o total de páginas e links HATEOAS
    /// para navegação entre as páginas.
    ///
    /// O tamanho de página permitido é de 1 a 100 registros.
    ///
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="page">
    /// Número da página solicitada. Valor padrão: 1.
    /// </param>
    /// <param name="pageSize">
    /// Quantidade de registros por página. Valor padrão: 10.
    /// </param>
    /// <response code="200">
    /// Listagem paginada retornada com sucesso.
    /// </response>
    /// <response code="400">
    /// Parâmetros de paginação inválidos.
    /// </response>
    /// <response code="401">
    /// Usuário não autenticado.
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
