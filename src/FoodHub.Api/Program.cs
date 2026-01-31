using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FoodHub.Api.Authentication;
using FoodHub.Api.GraphQL.Mutations;
using FoodHub.Api.GraphQL.Queries;
using FoodHub.Api.Middleware;
using FoodHub.Order.Application.Commands;
using FoodHub.Order.Application.Interfaces;
using FoodHub.Order.Application.Queries;
using FoodHub.Order.Infrastructure.Persistence.Cosmos;
using FoodHub.Order.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault Configuration (Production only)
if (!builder.Environment.IsDevelopment())
{
    var keyVaultEndpoint = builder.Configuration["KeyVault:Endpoint"];
    if (!string.IsNullOrEmpty(keyVaultEndpoint))
    {
        var secretClient = new SecretClient(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
        builder.Configuration.AddAzureKeyVault(secretClient, new Azure.Extensions.AspNetCore.Configuration.Secrets.AzureKeyVaultConfigurationOptions());
    }
}
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection("Cosmos"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection("GoogleAuth"));

var cosmosOptions = builder.Configuration.GetSection("Cosmos").Get<CosmosOptions>() ?? new CosmosOptions();
builder.Services.AddSingleton(new Microsoft.Azure.Cosmos.CosmosClient(cosmosOptions.Endpoint, cosmosOptions.Key));
builder.Services.AddSingleton<CosmosContext>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<CreateOrderCommand>();
builder.Services.AddScoped<UpdateOrderStatusCommand>();
builder.Services.AddScoped<GetOrderByIdQuery>();
builder.Services.AddScoped<GetOrdersByUserQuery>();

builder.Services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = true,
			ValidIssuer = jwtOptions.Issuer,
			ValidAudience = jwtOptions.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
		};
	});

builder.Services.AddAuthorization();

builder.Services
	.AddGraphQLServer()
	.AddAuthorization()
	.AddQueryType(d => d.Name("Query"))
	.AddTypeExtension<OrderQuery>()
	.AddMutationType(d => d.Name("Mutation"))
	.AddTypeExtension<OrderMutation>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.MapGet("/", () => "FoodHub Order Service is running");

app.Run();
