using AiTrainer.Web.Api.Auth;
using AiTrainer.Web.Api.Middlewares;
using AiTrainer.Web.Api.SignalR.Extensions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.Extensions;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.UserInfoClient;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
var appSettings = builder.Configuration.GetSection(ApplicationSettingsConfiguration.Key);

var useStaticFiles = bool.Parse(
    builder.Configuration.GetSection("UseStaticFiles")?.Value ?? "false"
);

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
        opts.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromSeconds(60) };
    })
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    );

builder.Services.AddAuthorizationServices(builder.Configuration, builder.Environment);

builder
    .Services.AddCoreClient(builder.Configuration)
    .AddSqlPersistence(builder.Configuration, builder.Environment.IsDevelopment())
    .AddUserInfoClient(builder.Configuration)
    .AddAiTrainerSignalR()
    .AddDomainModelServices()
    .AddDomainServices(builder.Configuration);

builder.Services.AddCors(p =>
    p.AddPolicy(
        "corsapp",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

            builder.WithOrigins("http://localhost:3000").AllowCredentials();
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
app.UseAuthentication();
app.UseAiTrainerDefaultMiddlewares();
app.MapAiTrainerSignalRHubs();
app.MapControllers();

await app.RunAsync();
