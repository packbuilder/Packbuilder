using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Options;
using Packbuilder.Services;

[assembly: InternalsVisibleTo("Packbuilder.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddIdentityCore<User>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddSingleton<JwtOptions>(pp =>
{
    return builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtOptions =>
{
    jwtOptions.Authority = "https://api.packbuilder.io";
    jwtOptions.Audience = "https://packbuilder.io";
});
builder.Services.AddDbContext<PackbuilderContext>(opt =>
    opt.UseInMemoryDatabase("PackbuilderDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
