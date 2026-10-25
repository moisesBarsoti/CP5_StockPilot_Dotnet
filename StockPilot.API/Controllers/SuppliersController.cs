
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StockPilot.Application.DTOs.Supplier;
using StockPilot.Application.Interfaces.Services;

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

    /// <summary>
    /// Cadastra um novo fornecedor.
    /// </summary>
    /// <remarks>
    /// Registra um fornecedor com nome, e-mail e CEP.
    ///
    /// Durante o cadastro, consulta a API externa ViaCEP para
    /// obter automaticamente a cidade e o estado do endereço.
    ///
    /// O e-mail é normalizado e não pode estar cadastrado
    /// para outro fornecedor.
    ///
    /// Operação permitida somente para usuários ADMIN.
    /// </remarks>
    /// <param name="request">
    /// Dados do fornecedor, incluindo nome, e-mail e CEP.
    /// </param>
    /// <response code="201">
    /// Fornecedor cadastrado com sucesso.
    /// </response>
    /// <response code="400">
    /// Dados inválidos ou CEP não encontrado.
    /// </response>
    /// <response code="401">
    /// Usuário não autenticado.
    /// </response>
    /// <response code="403">
    /// Usuário sem permissão para cadastrar fornecedores.
    /// </response>
    /// <response code="409">
    /// E-mail já cadastrado para outro fornecedor.
    /// </response>
    /// <response code="502">
    /// O ViaCEP retornou dados incompletos.
    /// </response>
    /// <response code="503">
    /// Serviço externo de consulta de CEP indisponível.
    /// </response>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
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

    /// <summary>
    /// Consulta um fornecedor pelo identificador.
    /// </summary>
    /// <remarks>
    /// Busca um fornecedor cadastrado no Oracle Database.
    ///
    /// Retorna as informações do fornecedor, incluindo
    /// nome, e-mail, CEP, cidade e estado.
    ///
    /// Caso o fornecedor não exista, o tratamento global de
    /// exceções retorna HTTP 404.
    ///
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">
    /// Identificador único do fornecedor (GUID).
    /// </param>
    /// <response code="200">
    /// Fornecedor encontrado com sucesso.
    /// </response>
    /// <response code="401">
    /// Usuário não autenticado.
    /// </response>
    /// <response code="404">
    /// Fornecedor não encontrado.
    /// </response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _supplierService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Data);
    }
}
