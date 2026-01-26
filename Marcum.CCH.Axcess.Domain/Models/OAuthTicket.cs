namespace Marcum.CCH.Axcess.Domain.Models;

public class OAuthTicket
{
    public string access_token { get; init; }
    public string refresh_token { get; init; }

    public DateTime? IssuedAt { get; set; }
    public DateTime? RefreshedAt { get; set; }

    public OAuthTicket()
    {
        access_token = string.Empty;
        refresh_token = string.Empty;
        IssuedAt = null;
        RefreshedAt = null;
    }

    public OAuthTicket(string _access_token, string _refresh_token, DateTime? _issuedAt = null, DateTime? _refreshedAt = null)
    {
        access_token = _access_token;
        refresh_token = _refresh_token;
        IssuedAt = _issuedAt;
        RefreshedAt = _refreshedAt;
    }
    public override string ToString()
    {
        return $"access_token: {access_token}, refresh_token: {refresh_token}, IssuedAt: {IssuedAt?.ToString() ?? "null"}, RefreshedAt: {RefreshedAt?.ToString() ?? "null"}";
    }
}
