using AiTrainer.Web.Api.Auth;
using AiTrainer.Web.Api.Middlewares;
using AiTrainer.Web.Common.Configuration;
using Microsoft.AspNetCore.Http.Timeouts;
using System.Text.Json;
using AiTrainer.Web.Api.Extensions;

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
        opts.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromSeconds(360) };
    })
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    );

builder.Services.AddAuthorizationServices(builder.Configuration, builder.Environment);

builder.Services.AddAiTrainerServices(builder.Configuration, builder.Environment);

builder.Services.AddCors(p =>
    p.AddPolicy(
        "corsapp",
        x =>
        {
            x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

            x.WithOrigins("http://localhost:3000").AllowCredentials();
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
// app.MapAiTrainerSignalRHubs();
app.MapControllers();

await app.RunAsync();
