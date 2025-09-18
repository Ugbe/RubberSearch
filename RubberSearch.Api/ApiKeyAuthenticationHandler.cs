using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using RubberSearch.Core.Utilities;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApiKeyMapper _apiKeyMapper;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ApiKeyMapper apiKeyMapper)
        : base(options, logger, encoder, clock)
    {
        _apiKeyMapper = apiKeyMapper;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var headerValue)) return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
        var key = headerValue.ToString().Split(' ').Last();
        var tenantId = _apiKeyMapper.GetTenantIdForApiKey(key);

        if (string.IsNullOrWhiteSpace(tenantId)) return Task.FromResult(AuthenticateResult.Fail("Invlaid API Key"));

        var claims = new[]
        {
            new Claim("TenantId", tenantId)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}