using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using morse_auth;
using morse_service.Database;
using morse_service.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

#region jwt configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(TokenParams.PublicKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = TokenParams.Issuer,
            ValidAudience = TokenParams.Audience,
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    });
#endregion

#region database context configuration
builder.Services.AddDbContext<MSDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

builder.Services.AddDbContextFactory<MSDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);
#endregion

#region handling dependency injection
void RegisterServicesFromAssembly(IServiceCollection services, Assembly assembly)
{
    var interfaces = assembly.GetTypes()
        .Where(t => t.IsInterface && t.Namespace == "morse_service.Interfaces.Services");

    foreach (var interfaceType in interfaces)
    {
        var implementation = assembly.GetTypes()
            .FirstOrDefault(t => t.IsClass &&
                               !t.IsAbstract &&
                               interfaceType.IsAssignableFrom(t) &&
                               t.Namespace == "morse_service.ServiceImplementations");

        if (implementation != null)
        {
            services.AddScoped(interfaceType, implementation);
        }
    }
}
RegisterServicesFromAssembly(builder.Services, Assembly.GetExecutingAssembly());
#endregion

#region cors configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("web-client", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});
#endregion

#region signalR configuration
builder.Services.AddSignalR();
#endregion

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("web-client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();