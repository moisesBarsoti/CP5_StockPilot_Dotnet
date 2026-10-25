using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StockPilot.IntegrationTests.Controllers;

public class SuppliersIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SuppliersIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetSupplier_WhenNotFound_ShouldReturnNotFound()
    {
        using var client = _factory.CreateClient();

        await AuthenticateAsync(client, "admin");

        var supplierId = Guid.NewGuid();

        using var response = await client.GetAsync(
            $"/api/suppliers/{supplierId}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task CreateSupplier_WithViewerToken_ShouldReturnForbidden()
    {
        using var client = _factory.CreateClient();

        await AuthenticateAsync(client, "viewer");

        var request = new
        {
            name = "Fornecedor Teste",
            email = "supplier.test@example.com",
            zipCode = "01001000"
        };

        using var response = await client.PostAsJsonAsync(
            "/api/suppliers",
            request
        );

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode
        );
    }

    private async Task AuthenticateAsync(
        HttpClient client,
        string username)
    {
        var configuration = _factory.Services
            .GetRequiredService<IConfiguration>();

        var key = username == "admin"
            ? "DemoAuth:AdminPassword"
            : "DemoAuth:ViewerPassword";

        var password = configuration[key];

        Assert.False(string.IsNullOrWhiteSpace(password));

        using var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { username, password }
        );

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync()
        );

        var token = json.RootElement
            .GetProperty("accessToken")
            .GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}
