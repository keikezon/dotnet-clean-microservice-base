using System.Security.Claims;
using System.Text;
using Common.Abstractions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Product.API.Mappings;
using Product.Application.Products.Abstractions;
using Product.Application.Products.Commands;
using Product.Application.Products.Validators;
using Product.Infrastructure.Messaging;
using Product.Infrastructure.Observability;
using Product.Infrastructure.Persistence;
using Product.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddAutoMapper(typeof(ProductProfile));

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
                  "Host=postgres;Port=5432;Database=productdb;Username=postgres;Password=postgres"));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddValidatorsFromAssembly(typeof(CreateProductValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(GetProductValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(DeleteProductValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UpdateProductValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UpdateStockValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(DecreaseStockValidator).Assembly);

builder.Services.AddScoped<CreateProduct.IHandler, CreateProduct.Handler>();
builder.Services.AddScoped<GetProduct.IHandler, GetProduct.Handler>();
builder.Services.AddScoped<DeleteProduct.IHandler, DeleteProduct.Handler>();
builder.Services.AddScoped<ListProduct.IHandler, ListProduct.Handler>();
builder.Services.AddScoped<UpdateProduct.IHandler, UpdateProduct.Handler>();
builder.Services.AddScoped<UpdateStock.IHandler, UpdateStock.Handler>();
builder.Services.AddScoped<DecreaseStock.IHandler, DecreaseStock.Handler>();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddMassTransitWithRabbit(builder.Configuration);

builder.Services.AddOpenTelemetryAll(serviceName: "product-api");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();      // logs no console
builder.Logging.AddDebug();        // logs no debug output
builder.Logging.AddEventSourceLogger(); // logs para EventSource

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