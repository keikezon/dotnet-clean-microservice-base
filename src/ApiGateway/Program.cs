using System.Text;
using Common.Abstractions;
using Common.Helpers;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Logs para debug
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Adiciona o YARP lendo do appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"]
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

var jwtModel = new Jwt
{
    Audience = builder.Configuration["Jwt:Audience"],
    Issuer = builder.Configuration["Jwt:Issuer"],
    Key = builder.Configuration["Jwt:Key"],
    SecurityAlgorithm = builder.Configuration["Jwt:SecurityAlgorithm"],

};

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrEmpty(token))
        {
            var jwtHelper = new JwtHelper(jwtModel.Key, jwtModel.Issuer, jwtModel.Audience);
            var userId = jwtHelper.ValidateToken(token);
            if (userId != null)
                context.Request.Headers["Authorization"] = token;
            else
                context.Request.Headers.Remove("Authorization");
        }

        await next();
    });
});

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();