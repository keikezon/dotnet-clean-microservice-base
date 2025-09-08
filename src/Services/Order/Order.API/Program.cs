using System.Security.Claims;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order.API.Mappings;
using Order.Application.Orders.Abstractions;
using Order.Application.Orders.Commands;
using Order.Application.Orders.Validators;
using Order.Infrastructure.Clients;
using Order.Infrastructure.Messaging;
using Order.Infrastructure.Observability;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddAutoMapper(typeof(OrderProfile));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres") ??
                  "Host=postgres;Port=5432;Database=orderdb;Username=postgres;Password=postgres"));

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(GetOrderValidator).Assembly);

builder.Services.AddScoped<CreateOrder.IHandler, CreateOrder.Handler>();
builder.Services.AddScoped<GetOrder.IHandler, GetOrder.Handler>();
builder.Services.AddScoped<ListOrder.IHandler, ListOrder.Handler>();

builder.Services.AddMassTransitWithRabbit(builder.Configuration);

builder.Services.AddOpenTelemetryAll(serviceName: "order-api");

builder.Services.AddHttpClient<IProductClient, ProductClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Api"]!);
});

builder.Services.AddHttpClient<IIdentityClient, IdentityServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Api"]!);
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<Common.ErrorHandling.ApiExceptionMiddleware>();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();