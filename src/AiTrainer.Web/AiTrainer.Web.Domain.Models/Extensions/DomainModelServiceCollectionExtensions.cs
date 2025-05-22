using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class DomainModelServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainModelServices(this IServiceCollection services)
        {
            services.AddModelValidators();

            return services;
        }

        private static IServiceCollection AddModelValidators(
            this IServiceCollection services
        )
        {
            services
                .AddSingleton<IValidator<SimilaritySearchInput>, SimilaritySearchInputValidator>()
                .AddSingleton<IValidator<User>, UserValidator>()
                .AddSingleton<IValidator<FileCollection>, FileCollectionValidator>()
                .AddSingleton<IValidator<FileDocument>, FileDocumentValidator>()
                .AddSingleton<IValidator<ChatGptFormattedQueryInput>, ChatGptFormattedQueryInputValidator>()
                .AddSingleton<IValidator<AnalyseChunkInReferenceToQuestionQueryInput>, AnalyseChunkInReferenceToQuestionQueryValidator>()
                .AddSingleton<IValidator<SharedFileCollectionMember>, SharedFileCollectionMemberValidator>()
                .AddSingleton<IValidator<IEnumerable<SharedFileCollectionMember>>>(sp => 
                    sp.GetRequiredService<IValidator<SharedFileCollectionMember>>()
                        .CreateEnumerableValidator());

            return services;
        }
    }
}
