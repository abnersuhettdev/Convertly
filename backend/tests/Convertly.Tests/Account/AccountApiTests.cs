using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Convertly.Application.Account.Dtos;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;
using Convertly.Infrastructure.Persistence;
using Convertly.Tests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Convertly.Tests.Account;

public sealed class AccountApiTests
{
    [Fact]
    public async Task ChangePassword_WithAuthentication_UpdatesPassword()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndReadAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        var response = await client.PatchAsJsonAsync(
            "/api/account/password",
            new ChangePasswordRequest("StrongPassword123!", "NewStrongPassword123!"));

        response.EnsureSuccessStatusCode();
        client.DefaultRequestHeaders.Authorization = null;

        var oldLogin = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("abner@example.com", "StrongPassword123!"));
        var newLogin = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("abner@example.com", "NewStrongPassword123!"));

        Assert.Equal(HttpStatusCode.Unauthorized, oldLogin.StatusCode);
        newLogin.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_ReturnsBadRequest()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndReadAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        var response = await client.PatchAsJsonAsync(
            "/api/account/password",
            new ChangePasswordRequest("wrong-password", "NewStrongPassword123!"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.Contains("Current password is invalid", body?.Errors ?? []);
    }

    [Fact]
    public async Task ChangePassword_WithInvalidNewPassword_ReturnsBadRequest()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndReadAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        var response = await client.PatchAsJsonAsync(
            "/api/account/password",
            new ChangePasswordRequest("StrongPassword123!", "short"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.Contains("New password must be at least 8 characters", body?.Errors ?? []);
    }

    [Fact]
    public async Task ChangePassword_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PatchAsJsonAsync(
            "/api/account/password",
            new ChangePasswordRequest("StrongPassword123!", "NewStrongPassword123!"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var response = await DeleteAccountAsync(client, new DeleteAccountRequest("StrongPassword123!"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_WithWrongPassword_ReturnsBadRequest()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndReadAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        var response = await DeleteAccountAsync(client, new DeleteAccountRequest("wrong-password"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.Contains("Current password is invalid", body?.Errors ?? []);
    }

    [Fact]
    public async Task DeleteAccount_WithValidPassword_DeactivatesUser()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndReadAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        var response = await DeleteAccountAsync(client, new DeleteAccountRequest("StrongPassword123!"));

        response.EnsureSuccessStatusCode();

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var user = await dbContext.Users.SingleAsync();

        Assert.False(user.IsActive);

        var meResponse = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, meResponse.StatusCode);
    }

    private static async Task<AuthResponse> RegisterAndReadAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("Abner Suhett", "Abner@example.com", "StrongPassword123!"));
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

        return body?.Data ?? throw new InvalidOperationException("Auth response was empty.");
    }

    private static Task<HttpResponseMessage> DeleteAccountAsync(HttpClient client, DeleteAccountRequest request)
    {
        var message = new HttpRequestMessage(HttpMethod.Delete, "/api/account")
        {
            Content = JsonContent.Create(request)
        };

        return client.SendAsync(message);
    }
}
