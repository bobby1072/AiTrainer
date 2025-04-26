using Microsoft.Extensions.Options;

namespace AiTrainer.Web.TestBase
{
    public class TestOptions<T> : IOptionsSnapshot<T>, IOptions<T>
        where T : class
    {
        public T Value { get; set; }

        public TestOptions(T value)
        {
            Value = value;
        }
        public T Get(string? stringKey)
        {
            return Value;
        }
    }
}
