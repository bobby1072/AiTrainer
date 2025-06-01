using AiTrainer.Web.Api.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.ChatGpt.Abstract;
using AiTrainer.Web.Domain.Services.ChatGpt.Concrete;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Domain.Services.User.Concrete;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Repositories.Concrete;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AiTrainer.Web.Api.Tests.ExtensionTests;

public sealed class AiTrainerServiceCollectionExtensionsTests
{
    private static readonly Dictionary<string, string?> _inMemorySettings = new()
    {
        // Logging
        ["Logging:LogLevel:Default"] = "Debug",
        ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning",

        // ApplicationSettings
        ["ApplicationSettings:Name"] = "AiTrainer.Web",
        ["ApplicationSettings:ReleaseVersion"] = "1.0",

        // AllowedHosts
        ["AllowedHosts"] = "*",

        // ConnectionStrings
        ["ConnectionStrings:PostgresConnection"] =
            "Server=test;Port=5560;Database=test;User ID=test;Password=test;SslMode=Disable",

        // DbMigrations
        ["DbMigrations:StartVersion"] = "1",
        ["DbMigrations:TotalAttempts"] = "2",
        ["DbMigrations:DelayBetweenAttemptsInSeconds"] = "1",

        // ClientSettings
        ["ClientSettings:Scope"] = "test",
        ["ClientSettings:InternalAuthorityHost"] = "https://test:44363",
        ["ClientSettings:InternalUserInfoEndpoint"] = "https://test:44363/connect/userinfo",
        ["ClientSettings:AuthorityClientId"] = "test",

        // UserInfoClient
        ["UserInfoClient:TimeoutInSeconds"] = "15",
        ["UserInfoClient:TotalAttempts"] = "2",
        ["UserInfoClient:DelayBetweenAttemptsInSeconds"] = "1",

        // FaissSyncRetrySettings
        ["FaissSyncRetrySettings:TotalAttempts"] = "2",
        ["FaissSyncRetrySettings:DelayBetweenAttemptsInSeconds"] = "1",
        ["FaissSyncRetrySettings:UseRetry"] = "true",

        // AiTrainerCore
        ["AiTrainerCore:DocumentChunkingType"] = "1",
        ["AiTrainerCore:ApiKey"] = "test",
        ["AiTrainerCore:BaseEndpoint"] = "http://test:8000",
        ["AiTrainerCore:TimeoutInSeconds"] = "300",
        ["AiTrainerCore:TotalAttempts"] = "2",
        ["AiTrainerCore:DelayBetweenAttemptsInSeconds"] = "5"
    };
    private readonly Mock<IWebHostEnvironment> _mockWebHost = new();
    private readonly IServiceCollection _serviceCollectionWithAiTrainerServicesAdded;
    
    public AiTrainerServiceCollectionExtensionsTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(_inMemorySettings)
            .Build();
        _mockWebHost.Setup(x => x.EnvironmentName).Returns("Development");
        
        _serviceCollectionWithAiTrainerServicesAdded = new ServiceCollection()
            .AddAiTrainerServices(config, _mockWebHost.Object);
    }
    
    
    [Fact]
    public void AddCoreClient_Registers_Expected_HttpClients()
    {
        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(ICoreClient<CoreUpdateFaissStoreInput, CoreFaissStoreResponse>) &&
            d.Lifetime == ServiceLifetime.Transient);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(ICoreClient<CoreSimilaritySearchInput, CoreSimilaritySearchResponse>) &&
            d.Lifetime == ServiceLifetime.Transient);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(ICoreClient<CoreClientHealthResponse>) &&
            d.Lifetime == ServiceLifetime.Transient);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse>) &&
            d.Lifetime == ServiceLifetime.Transient);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse>) &&
            d.Lifetime == ServiceLifetime.Transient);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(ICoreClient<CoreDocumentToChunkInput, CoreChunkedDocumentResponse>) &&
            d.Lifetime == ServiceLifetime.Transient);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse>) &&
            d.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void AddSqlPersistence_Registers_Repositories_And_DbContext()
    {
        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IRepository<UserEntity, Guid, User>) &&
            d.ImplementationType == typeof(UserRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileDocumentRepository) &&
            d.ImplementationType == typeof(FileDocumentRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileCollectionRepository) &&
            d.ImplementationType == typeof(FileCollectionRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileCollectionFaissRepository) &&
            d.ImplementationType == typeof(FileCollectionFaissRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IRepository<FileDocumentMetaDataEntity, long, FileDocumentMetaData>) &&
            d.ImplementationType == typeof(FileDocumentMetaDataRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IRepository<GlobalFileCollectionConfigEntity, long, GlobalFileCollectionConfig>) &&
            d.ImplementationType == typeof(GlobalFileCollectionConfigRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IRepository<SharedFileCollectionMemberEntity, Guid, SharedFileCollectionMember>) &&
            d.ImplementationType == typeof(SharedFileCollectionMemberRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IDbContextFactory<AiTrainerContext>) &&
            d.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddUserInfoClient_Registers_HttpClient()
    {
        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IUserInfoClient) &&
            d.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void AddDomainModelServices_Registers_Validators_AsSingletons()
    {
        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IValidator<SimilaritySearchInput>) &&
            d.ImplementationType == typeof(SimilaritySearchInputValidator) &&
            d.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IValidator<User>) &&
            d.ImplementationType == typeof(UserValidator) &&
            d.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IValidator<FileCollection>) &&
            d.ImplementationType == typeof(FileCollectionValidator) &&
            d.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IValidator<FileDocument>) &&
            d.ImplementationType == typeof(FileDocumentValidator) &&
            d.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IValidator<ChatGptFormattedQueryInput>) &&
            d.ImplementationType == typeof(ChatGptFormattedQueryInputValidator) &&
            d.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IValidator<AnalyseChunkInReferenceToQuestionQueryInput>) &&
            d.ImplementationType == typeof(AnalyseChunkInReferenceToQuestionQueryValidator) &&
            d.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IValidator<SharedFileCollectionMember>) &&
            d.ImplementationType == typeof(SharedFileCollectionMemberValidator) &&
            d.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IValidator<IEnumerable<SharedFileCollectionMember>>) &&
            d.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddDomainServices_Registers_Managers_And_HostedServices()
    {
        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IUserProcessingManager) &&
            d.ImplementationType == typeof(UserProcessingManager) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileCollectionProcessingManager) &&
            d.ImplementationType == typeof(FileCollectionProcessingManager) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileDocumentProcessingManager) &&
            d.ImplementationType == typeof(FileDocumentProcessingManager) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileCollectionFaissSimilaritySearchProcessingManager) &&
            d.ImplementationType == typeof(FileCollectionFaissSimilaritySearchProcessingManager) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileCollectionFaissRemoveDocumentsProcessingManager) &&
            d.ImplementationType == typeof(FileCollectionFaissRemoveDocumentsProcessingManager) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileCollectionFaissSyncProcessingManager) &&
            d.ImplementationType == typeof(FileCollectionFaissSyncProcessingManager) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IFileCollectionFaissSyncBackgroundJobQueue) &&
            d.ImplementationType == typeof(FileCollectionFaissBackgroundJobQueue) &&
            d.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IHealthProcessingManager) &&
            d.ImplementationType == typeof(HealthProcessingManager) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(ICachingService) &&
            d.ImplementationType == typeof(DistributedCachingService) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IChatGptQueryProcessingManager) &&
            d.ImplementationType == typeof(ChatGptQueryProcessingManager) &&
            d.Lifetime == ServiceLifetime.Scoped);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ServiceType == typeof(IHttpDomainServiceActionExecutor) &&
            d.ImplementationType == typeof(HttpDomainServiceActionExecutor) &&
            d.Lifetime == ServiceLifetime.Transient);

        Assert.Contains(_serviceCollectionWithAiTrainerServicesAdded, d =>
            d.ImplementationType == typeof(FileCollectionFaissBackgroundJobService) && d.Lifetime == ServiceLifetime.Singleton);
    }
}