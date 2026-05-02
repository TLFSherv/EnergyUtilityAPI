using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using EnergyUtilityApi;
public class ApiKeyHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly EnergyUtilityDbContext _context;
    private readonly IMemoryCache _cache;

    public ApiKeyHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        EnergyUtilityDbContext context,
        IMemoryCache cache) : base(options, logger, encoder)
    {
        _context = context;
        // implement caching
        _cache = cache;
    }
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
            return AuthenticateResult.Fail("API Key was not provided");

        // check if api key is in cache
        if (!_cache.TryGetValue($"apikey:{extractedApiKey.ToString()}", out ApiKeyLookup keyEntry))
        {
            keyEntry = await _context.UserApiKeys
                .Where(x => x.ApiKey == extractedApiKey.ToString())
                .Select(x => new ApiKeyLookup
                {
                    UserId = x.UserId,
                    ApiKey = x.ApiKey,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();

            if (keyEntry == null)
                return AuthenticateResult.Fail("Invalid API Key");

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(20));

            _cache.Set($"apiKey:{keyEntry.ApiKey}", keyEntry, cacheEntryOptions);
        }

        // Create the identity
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, keyEntry.UserId.ToString()) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}