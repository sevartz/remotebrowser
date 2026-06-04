using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
public static class ClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var subClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(subClaim, out var userId) ? userId : throw new InvalidOperationException("User ID claim is missing or invalid.");
    }
}