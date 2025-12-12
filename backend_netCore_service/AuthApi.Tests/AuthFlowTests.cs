using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuthApi.Models;
using FluentAssertions;

namespace AuthApi.Tests;

public class AuthFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_WithValidAdminCredentials_ReturnsToken()
    {
        using var client = _factory.CreateClient();
        var request = new LoginRequest("admin@example.com", "Admin123!");

        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();
        payload.Should().NotBeNull();
        payload!.Role.Should().Be("Admin");
        payload.Access_token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var request = new LoginRequest("admin@example.com", "wrong");

        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task User_CannotAccess_AdminEndpoint()
    {
        using var client = _factory.CreateClient();

        var token = await GetTokenAsync(client, "user@example.com", "User123!");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/admin/reports");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        response = await client.GetAsync("/api/users/profile");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Admin_CanAccess_ProtectedEndpoints()
    {
        using var client = _factory.CreateClient();

        var token = await GetTokenAsync(client, "admin@example.com", "Admin123!");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var adminResponse = await client.GetAsync("/api/admin/reports");
        adminResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = await client.GetAsync("/api/users/profile");
        userResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task<string> GetTokenAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(username, password));
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return payload!.Access_token;
    }
}
