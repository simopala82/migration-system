using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Migration.API.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMigrationLogicServices()
    .AddMigrationDataAccessServices(builder);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLogging();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Usually set to 'true' for security
            ValidateIssuer = false, 
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ??
                throw new KeyNotFoundException("Jwt:Key missing in configuration file")
            )),
            ValidateLifetime = true 
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
    app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
