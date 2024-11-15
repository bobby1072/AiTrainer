using AiTrainer.Web.Api.Auth;
using AiTrainer.Web.Api.Middlewares;
using AiTrainer.Web.CoreClient;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services;
using AiTrainer.Web.Persistence;
using AiTrainer.Web.UserInfoClient;
using Microsoft.AspNetCore.Http.Timeouts;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
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

builder.Services.AddAuthorizationServices(builder.Configuration, builder.Environment);

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

await app.RunAsync();
