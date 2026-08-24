using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Perfume_King.API.DbContexts;
using Perfume_King.API.Models;
using Perfume_King.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "blazorWasmPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7147",
                           "https://localhost:7075")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();

    // Injecting values provided by your Cloudinary dashboard console panel
    var cloudName = "tod4wqqa";
    var apiKey = "635279573621668";

    // Replace this placeholder string with the actual secret you copied 
    var apiSecret = Environment.GetEnvironmentVariable("CloudinaryApiSecret") ?? "cCQL6VpvzH25-0s05D3JRosyeEQ";

    return new CloudinaryClient(httpClient, cloudName, apiKey, apiSecret);
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("PerfumeKingDb"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Convert.FromBase64String(Environment.GetEnvironmentVariable("JWT_KEY")!)
        ),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"FAIL: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
try
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    //Console.WriteLine("--> Applying pending database migrations...");
    //await dbContext.Database.MigrateAsync();
    //Console.WriteLine("--> Migrations applied successfully!");

    var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME");
    var adminHashedPassword = Environment.GetEnvironmentVariable("ADMIN_HASHED_PASSWORD");
    var adminSaltRawString = Environment.GetEnvironmentVariable("ADMIN_SALT");

    if (!string.IsNullOrEmpty(adminUsername) &&
        !string.IsNullOrEmpty(adminHashedPassword) &&
        !string.IsNullOrEmpty(adminSaltRawString))
    {
        byte[] adminSaltBytes = [.. adminSaltRawString
            .Replace("[", "")
            .Replace("]", "")
            .Replace(" ", "")
            .Split(',')
            .Select(byte.Parse)];

        var adminExists = await dbContext.AdminUsers.AnyAsync(a => a.Username == adminUsername);
        if (!adminExists)
        {
            var newAdmin = new AdminUser
            {
                Username = adminUsername,
                Password = adminHashedPassword,
                Salt = adminSaltBytes
            };

            dbContext.AdminUsers.Add(newAdmin);
            await dbContext.SaveChangesAsync();

            Console.WriteLine("--> Admin account seeded successfully into the database.");
        }
    }
    else
    {
        throw new Exception("ADMIN USER DATA IS MISSING FROM ENVIRONMENT VARIABLES!");
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"--> CRITICAL STARTUP ERROR: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"--> Inner Exception: {ex.InnerException.Message}");
    }
    Console.ResetColor();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("blazorWasmPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
