using System.Text.Json;
using AiTrainer.Web.Api.Auth;
using AiTrainer.Web.Api.Middlewares;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services;
using AiTrainer.Web.Persistence;
using AiTrainer.Web.UserInfoClient;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
var appSettings = builder.Configuration.GetSection(ApplicationSettingsConfiguration.Key);

var useStaticFiles = bool.Parse(
    builder.Configuration.GetSection("UseStaticFiles")?.Value ?? "false"
);

if (!appSettings.Exists())
{
    throw new Exception("ApplicationSettingsConfigurationS not found in configuration");
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

builder.Services.AddAuthorizationServices(builder.Configuration, builder.Environment);

builder
    .Services.AddCoreClient(builder.Configuration)
    .AddSqlPersistence(builder.Configuration)
    .AddUserInfoClient(builder.Configuration)
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
app.AddAiTrainerDefaultMiddlewares();

app.MapControllers();
#pragma warning disable ASP0014
if (useStaticFiles)
{
    app.UseEndpoints(endpoint =>
    {
        endpoint.MapFallbackToFile("index.html");
    });
#pragma warning restore ASP0014
    app.UseStaticFiles();
    app.UseSpa(spa =>
    {
        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                var headers = context.Context.Response.GetTypedHeaders();
                if (context.File.Name.EndsWith(".html"))
                {
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                        NoStore = true,
                        MustRevalidate = true,
                        MaxAge = TimeSpan.Zero,
                    };
                }
                else
                {
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        Private = false,
                        NoCache = false,
                        NoStore = false,
                        MaxAge = TimeSpan.FromDays(365),
                    };
                }
            },
        };
    });
}

await app.RunAsync();
