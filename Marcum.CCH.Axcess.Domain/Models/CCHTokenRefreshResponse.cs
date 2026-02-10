
namespace Marcum.CCH.Axcess.Domain.Models;

public record CchTokenRefreshResponse(
    string id_token,
    string access_token,
    int expires_in,
    string token_type,
    string refresh_token
    );
