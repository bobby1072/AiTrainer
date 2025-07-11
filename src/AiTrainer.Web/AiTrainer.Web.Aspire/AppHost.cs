using Projects;

var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<AiTrainer_Web_Api>(nameof(AiTrainer_Web_Api))
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();