using FluentValidation;
using Gringotts.Api.Features.UserAuthentication.Services;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddEndpoints();


builder.Services.AddSingleton<UserJwtService>();
builder.Services.AddSingleton<UserSecretService>();
builder.Services.AddSingleton<UserService>();

if (builder.Environment.IsDevelopment()) {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapEndpoints();

app.Run();
