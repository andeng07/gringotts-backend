using Gringotts.Api.Shared.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();

if (builder.Environment.IsDevelopment()) {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapEndpoints();

app.Run();
