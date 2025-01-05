using FluentValidation;
using Gringotts.Api.Features.Authentication.Services;
using Gringotts.Api.Features.Reader.Services;
using Gringotts.Api.Features.User.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetValue<string>("Database:ConnectionString")));

builder.Services.AddScoped<JwtService>();
builder.Services.AddSingleton<HashingService>();

builder.Services.AddScoped<UserSecretService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<ReaderService>();
builder.Services.AddScoped<LocationService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer", // Use "Bearer" for OAuth2 tokens
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' followed by your token."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                []
            }
        });
    });
}

builder.Services.AddEndpoints();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapEndpoints(app.MapGroup("api"));

app.Run();