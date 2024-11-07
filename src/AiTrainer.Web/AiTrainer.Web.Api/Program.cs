using AiTrainer.Web.Api.Middlewares;
using AiTrainer.Web.CoreClient;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services;
using AiTrainer.Web.Persistence;
using AiTrainer.Web.UserInfoClient;
using BT.Common.WorkflowActivities;
using Microsoft.AspNetCore.Http.Timeouts;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);


builder.Services
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddResponseCompression()
    .AddRequestTimeouts(opts =>
    {
        opts.DefaultPolicy = new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromSeconds(60),
        };
    })
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    );


builder.Services
    .AddCoreClient(builder.Configuration)
    .AddSqlPersistence(builder.Configuration)
    .AddWorkflowServices()
    .AddUserInfoClient()
    .AddDomainModelServices()
    .AddDomainServices();


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

app
    .UseMiddleware<ExceptionHandlingMiddleware>()
    .UseMiddleware<CorrelationIdMiddleware>();

app.MapControllers();

await app.RunAsync();
