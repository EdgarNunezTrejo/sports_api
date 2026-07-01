using Microsoft.EntityFrameworkCore;
using sports_api.Data;
using sports_api.Repositories;
using sports_api.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add repositories
builder.Services.AddScoped<TeamRepository>();
builder.Services.AddScoped<LeagueRepository>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<PositionRepository>();
builder.Services.AddScoped<SportRepository>();
builder.Services.AddScoped<MatchRepository>();
builder.Services.AddScoped<MatchEventRepository>();
builder.Services.AddScoped<ConversationRepository>();
builder.Services.AddScoped<ChatMessageRepository>();

// Add services
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<LeagueService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<PositionService>();
builder.Services.AddScoped<SportService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddScoped<MatchEventService>();
builder.Services.AddScoped<ConversationService>();
// DB Context
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "sports_api v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
