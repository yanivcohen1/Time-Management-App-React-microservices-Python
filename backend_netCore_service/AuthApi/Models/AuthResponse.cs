namespace AuthApi.Models;

public record AuthResponse(string Access_token, string Username, string Role, DateTime ExpiresAtUtc);
