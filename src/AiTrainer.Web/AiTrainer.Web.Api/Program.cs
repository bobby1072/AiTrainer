using System.Text.Json;
using AiTrainer.Web.Api.Middlewares;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.Extensions;
using AiTrainer.Web.Persistence;
using Microsoft.AspNetCore.Http.Timeouts;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
var appSettings = builder.Configuration.GetSection(ApplicationSettingsConfiguration.Key);

if (!appSettings.Exists())
{
    throw new Exception("ApplicationSettingsConfiguration not found in configuration");
}
builder.Services.Configure<ApplicationSettingsConfiguration>(appSettings);

builder
    .Services.AddDistributedMemoryCache()
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddResponseCompression()
    .AddRequestTimeouts(opts =>
    {
        opts.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromSeconds(90) };
    })
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    );

builder
    .Services.AddCoreClient(builder.Configuration)
    .AddSqlPersistence(builder.Configuration)
    .AddDomainModelServices()
    .AddDomainServices(builder.Configuration);

builder.Services.AddCors(p =>
    p.AddPolicy(
        "corsapp",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    )
);

var app = builder.Build();

await app.UseHangfireAsync();

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
app.AddAiTrainerDefaultMiddlewares();

app.MapControllers();

await app.RunAsync();
