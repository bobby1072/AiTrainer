using AiTrainer.Web.TestBase;
using Microsoft.Net.Http.Headers;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public abstract class DomainServiceTestBase: AiTrainerTestBase
    {
        protected void AddAccessTokenToRequestHeaders()
        {
            _headerDictionary[HeaderNames.Authorization] = $"Bearer {Guid.NewGuid()}";
        }
    }
}
