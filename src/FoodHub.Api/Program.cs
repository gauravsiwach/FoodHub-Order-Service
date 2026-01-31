using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

//using FoodHub.Api.Authentication;
using FoodHub.Api.GraphQL.Mutations;
using FoodHub.Api.GraphQL.Queries;
using FoodHub.Api.Middleware;
using FoodHub.Order.Application.Commands;
using FoodHub.Order.Application.Interfaces;
using FoodHub.Order.Application.Queries;
using FoodHub.Order.Infrastructure.Persistence.Cosmos;
using FoodHub.Order.Infrastructure.Persistence.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault Configuration (Production only)
//if (!builder.Environment.IsDevelopment())
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
//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
//builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection("GoogleAuth"));

var cosmosOptions = builder.Configuration.GetSection("Cosmos").Get<CosmosOptions>() ?? new CosmosOptions();
builder.Services.AddSingleton(new Microsoft.Azure.Cosmos.CosmosClient(cosmosOptions.Endpoint, cosmosOptions.Key));
builder.Services.AddSingleton<CosmosContext>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<CreateOrderCommand>();
builder.Services.AddScoped<UpdateOrderStatusCommand>();
builder.Services.AddScoped<GetOrderByIdQuery>();
builder.Services.AddScoped<GetOrdersByUserQuery>();

//builder.Services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var secret = jwtSection.GetValue<string>("Secret") ?? throw new InvalidOperationException("JWT Secret is not configured");
var issuer = jwtSection.GetValue<string>("Issuer") ?? "FoodHub";
var audience = jwtSection.GetValue<string>("Audience") ?? "FoodHub";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Serilog.Log.Error(context.Exception, "JWT Authentication failed: {Message}", context.Exception.Message);
                return System.Threading.Tasks.Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Serilog.Log.Information("JWT Token successfully validated for: {Name}", context.Principal?.Identity?.Name);
                return System.Threading.Tasks.Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Serilog.Log.Warning("JWT Challenge triggered: {Error} - {ErrorDescription}", context.Error, context.ErrorDescription);
                return System.Threading.Tasks.Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

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
// Inline middleware to log user identity and claims
app.Use(async (context, next) =>
{
    var user = context.User;
    if (user?.Identity != null)
    {
        Serilog.Log.Information("User.Identity.IsAuthenticated: {IsAuthenticated}", user.Identity.IsAuthenticated);
        Serilog.Log.Information("User.Identity.Name: {Name}", user.Identity.Name);
        foreach (var claim in user.Claims)
        {
            Serilog.Log.Information("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }
    }
    else
    {
        Serilog.Log.Information("No user identity present on request.");
    }
    await next();
});


app.MapGraphQL("/graphql");
 

app.Run();
