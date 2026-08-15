using Microsoft.Azure.Cosmos;
using TaskService.Api.Application.Interfaces;
using TaskService.Api.Infrastructure.CosmosDb;
using TaskService.Api.Middleware;
using TaskService.Api.Options;
using TaskService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<CosmosDbOptions>(
    builder.Configuration.GetSection(CosmosDbOptions.SectionName));

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<CosmosDbOptions>>().Value;

    return new CosmosClient(
        options.Endpoint,
        options.Key,
        new CosmosClientOptions { ApplicationName = "TaskService" });
});

builder.Services.AddSingleton<ITaskRepository, CosmosTaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService.Api.Application.Services.TaskService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection(); 
app.MapControllers();
app.Run();

public partial class Program { }
