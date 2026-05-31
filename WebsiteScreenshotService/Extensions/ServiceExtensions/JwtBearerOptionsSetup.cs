using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebsiteScreenshotService.Configurations;

using TokenTypes = WebsiteScreenshotService.Constants.Claims.TokenTypes;

namespace WebsiteScreenshotService.Extensions.ServiceExtensions;

public class JwtBearerOptionsSetup(IOptions<AuthorizationConfiguration> config) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthorizationConfiguration _config = config.Value;

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
            return;

        //options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = _config.Issuer,
            ValidAudience = _config.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.Secret)),

            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = Constants.Claims.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var auth = context.Request.Headers.Authorization.FirstOrDefault();

                if (string.IsNullOrEmpty(auth))
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }

                if (!auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }

                var token = auth["Bearer ".Length..];

                if (string.IsNullOrEmpty(token))
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }

                context.Token = token["api_v1_".Length..];
                context.HttpContext.SetRawAuthToken(token);

                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                var typeClaim = context.Principal?.GetTokenType();

                if (typeClaim != TokenTypes.Authorization && typeClaim != TokenTypes.Api)     
                    context.Fail("Invalid token type");

                return Task.CompletedTask;
            }
        };
    }

    public void Configure(JwtBearerOptions options)
        => Configure(Options.DefaultName, options);
}