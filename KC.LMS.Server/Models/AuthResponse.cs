namespace KC.LMS.Server.Models
{
    public record AuthResponse(string Token, DateTimeOffset ExpiresAt, Guid UserId, string Email, string UserName);

}
