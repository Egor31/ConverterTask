using System.Threading.Channels;
using Backend.Api;
using Backend.Api.Models;
using Backend.Api.Repositories;
using Backend.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton(
    Channel.CreateUnbounded<ConvertFileTask>(
        new UnboundedChannelOptions() { SingleReader = true }));
builder.Services.AddSingleton(svc => svc.GetRequiredService<Channel<ConvertFileTask>>().Reader);
builder.Services.AddSingleton(svc => svc.GetRequiredService<Channel<ConvertFileTask>>().Writer);

builder.Services.AddScoped<FileConverterRepository>();
builder.Services.AddScoped<FileConverterService>();

builder.Services.AddHostedService<FileConverterHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

app.MapHub<ConvertTaskHub>("/taskCompletedHub");
app.MapControllers();

await StartupDatabaseManagement.InitializeTables(app);

app.Run();
