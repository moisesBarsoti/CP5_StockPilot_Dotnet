
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StockPilot.IntegrationTests.Controllers;

public class ProductsIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductsIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProducts_WithoutToken_ShouldReturnUnauthorized()
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync("/api/products");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [Fact]
    public async Task CreateProduct_WithViewerToken_ShouldReturnForbidden()
    {
        using var client = _factory.CreateClient();

        var password = _factory.Services
            .GetRequiredService<IConfiguration>()
            ["DemoAuth:ViewerPassword"];

        Assert.False(string.IsNullOrWhiteSpace(password));

        var token = await LoginAsync(client, "viewer", password!);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.PostAsJsonAsync(
            "/api/products",
            new
            {
                name = "Produto não autorizado",
                price = 100m,
                stockQuantity = 5,
                supplierId = Guid.NewGuid()
            }
        );

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode
        );
    }

    [Fact]
    public async Task GetProducts_WithAdminToken_ShouldReturnOk()
    {
        using var client = _factory.CreateClient();

        var password = _factory.Services
            .GetRequiredService<IConfiguration>()
            ["DemoAuth:AdminPassword"];

        Assert.False(string.IsNullOrWhiteSpace(password));

        var token = await LoginAsync(client, "admin", password!);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.GetAsync(
            "/api/products?page=1&pageSize=10"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync()
        );

        var items = json.RootElement
            .GetProperty("data")
            .GetProperty("items");

        Assert.Equal(JsonValueKind.Array, items.ValueKind);
    }

    private static async Task<string> LoginAsync(
        HttpClient client,
        string username,
        string password)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { username, password }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync()
        );

        return json.RootElement
            .GetProperty("accessToken")
            .GetString()!;
    }
}
