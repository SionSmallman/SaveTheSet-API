using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPI.Web;
using sts_net.Data;
using sts_net.Data.Repositories;
using sts_net.Data.Repositories.Interfaces;
using sts_net.Services;
using sts_net.Services.Interfaces;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
            .WithOrigins("http://localhost:5173","https://sts.sionsmallman.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

// Add services to the container.
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                });
builder.Services.AddHttpClient();

builder.Services.AddSingleton<ISpotifyClientCredentialsContext, SpotifyClientCredentialsContext>();
builder.Services.AddScoped<ISpotifyClientFactory, SpotifyClientFactory>();
builder.Services.AddScoped<IStsTokenService, StsTokenService>();
builder.Services.AddScoped<ISetlistService, SetlistService>();
builder.Services.AddScoped<ISpotifyClientCredentialsServiceFactory, SpotifyClientCredentialsServiceFactory>();
builder.Services.AddScoped<IOAuthClient, OAuthClient>();

builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });

builder.Services.AddAuthorization();

// Initialise DB context
var connectionString = builder.Configuration.GetConnectionString("Db");
builder.Services.AddDbContext<SaveTheSetContext>(options =>

    options.UseNpgsql(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply middleware to specific routes.
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/playlists"), appBuilder => { app.UseRefreshSpotifyToken(); });

app.Run();
