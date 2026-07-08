using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Interfaces;
using sports_api.Repositories;
using sports_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using sports_api.Models;
using sports_api.Hubs;
using CloudinaryDotNet;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR.
builder.Services.AddSignalR();
// Add Cloudinary configuration
var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");
var account = new Account(
    cloudinaryConfig["CloudName"],
    cloudinaryConfig["ApiKey"],
    cloudinaryConfig["ApiSecret"]
);

var cloudinary = new Cloudinary(account) { Api = { Secure = true } };
builder.Services.AddSingleton(cloudinary);

var port = Environment.GetEnvironmentVariable("PORT");
if (port != null)
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Add repositories
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<ISportRepository, SportRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchEventRepository, MatchEventRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<LeagueService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<PositionService>();
builder.Services.AddScoped<SportService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddScoped<MatchEventService>();
builder.Services.AddScoped<ConversationService>();
builder.Services.AddScoped<AuthService>();


// DB Context
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, token) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Ingresa tu JWT token"
        };
        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, token) =>
    {
        var requirement = new OpenApiSecurityRequirement();
        requirement.Add(new OpenApiSecuritySchemeReference("Bearer"), []);
        operation.Security = [requirement];
        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await db.Database.MigrateAsync();

    var adminEmail = config["Seed:AdminEmail"];
    var adminPassword = config["Seed:AdminPassword"];

    if (adminEmail != null && adminPassword != null)
    {
        if (!db.Users.Any(u => u.Email == adminEmail))
        {
            db.Users.Add(new User
            {
                Email = adminEmail.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                Role = UserRole.Admin,
                Provider = AuthProvider.Email
            });
            await db.SaveChangesAsync();
        }
    }
}

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "sports_api v1"));
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
