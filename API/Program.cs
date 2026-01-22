using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyAPP.API.Services;
using MyAPP.Common;
using MyAPP.Driver.DB;
using MyAPP.Driver.MariaDB;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile(
    "config/config.json",
    optional: false,
    reloadOnChange: true
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyAPP API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен в формате: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtService>();
var dbType = builder.Configuration["DBType"];
Console.WriteLine($"DBType = '{dbType}'");
switch (dbType)
{
    case "MariaDB":
        builder.Services.AddScoped<IRefreshToken, MariaDBRefreshTokens>();
        Console.WriteLine("✅ Using MariaDB");
        break;

    case "SQLite":
        builder.Services.AddScoped<IRefreshToken, DBRefreshTokens>();
        Console.WriteLine("✅ Using SQLite");
        break;

    default:
        throw new Exception($"Unsupported DBType: {dbType}");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();
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
