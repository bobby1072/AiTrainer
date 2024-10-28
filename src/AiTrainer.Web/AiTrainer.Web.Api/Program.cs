using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient;
using AiTrainer.Web.Persistence;
using Microsoft.AspNetCore.Http.Timeouts;
using System.Text.Json;
using BT.Common.WorkflowActivities;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);


builder
    .Services.AddHttpClient()
    .AddHttpContextAccessor()
    .AddResponseCompression()
    .AddRequestTimeouts(opts =>
    {
        opts.DefaultPolicy = new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromMilliseconds(60000),
        };
    })
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    );


builder.Services.AddCoreClient(builder.Configuration);

builder.Services.AddSqlPersistence(builder.Configuration);

builder.Services.AddWorkflowServices();

builder.Services.AddCors(p =>
    p.AddPolicy(
        "corsapp",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("corsapp");
}
else
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseResponseCompression();
app.UseAuthorization();
app.MapControllers();

app.Run();
