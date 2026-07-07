using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Interfaces;
using sports_api.Repositories;
using sports_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
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
app.UseAuthorization();
app.UseAuthentication();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "sports_api v1"));
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
