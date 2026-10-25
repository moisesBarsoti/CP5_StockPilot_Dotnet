
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using StockPilot.Application.Common.Results;
using StockPilot.Application.DTOs.External;
using StockPilot.Application.Interfaces.External;

namespace StockPilot.Infrastructure.External;

public class ViaCepService : IZipCodeService
{
    private readonly HttpClient _httpClient;

    public ViaCepService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OperationResult<ZipCodeAddress>> FindAsync(
        string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode)
            || zipCode.Length != 8
            || !zipCode.All(char.IsDigit))
        {
            return OperationResult<ZipCodeAddress>.Failure(
                "INVALID_ZIP_CODE",
                "O CEP deve conter 8 dígitos."
            );
        }

        try
        {
            var response = await _httpClient
                .GetFromJsonAsync<ViaCepResponse>(
                    $"{zipCode}/json/"
                );

            if (response is null ||
                response.Erro.ToString()
                    .Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return OperationResult<ZipCodeAddress>.Failure(
                    "ZIP_CODE_NOT_FOUND",
                    "CEP não encontrado."
                );
            }

            if (string.IsNullOrWhiteSpace(response.City)
                || string.IsNullOrWhiteSpace(response.State))
            {
                return OperationResult<ZipCodeAddress>.Failure(
                    "INVALID_EXTERNAL_RESPONSE",
                    "O ViaCEP retornou dados incompletos."
                );
            }

            return OperationResult<ZipCodeAddress>.Success(
                new ZipCodeAddress(
                    response.City,
                    response.State
                )
            );
        }
        catch (Exception ex) when (
            ex is HttpRequestException
                or TaskCanceledException
                or JsonException)
        {
            return OperationResult<ZipCodeAddress>.Failure(
                "EXTERNAL_SERVICE_UNAVAILABLE",
                "Não foi possível consultar o ViaCEP."
            );
        }
    }

    private sealed class ViaCepResponse
    {
        [JsonPropertyName("localidade")]
        public string? City { get; set; }

        [JsonPropertyName("uf")]
        public string? State { get; set; }

        [JsonPropertyName("erro")]
        public JsonElement Erro { get; set; }
    }
}
