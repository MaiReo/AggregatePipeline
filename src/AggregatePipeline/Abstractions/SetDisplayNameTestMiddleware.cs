using System;
using System.Threading.Tasks;
using AggregatePipeline.DependencyInjection;

namespace AggregatePipeline.Abstractions {
    public class SetDisplayNameTestMiddleware : IAsyncPipelineContextHandler<TestContextBase>, ILifeStyleTransient {
        public async Task HandleAsync (TestContextBase testContext, Func<Task> next) {
            Console.WriteLine ($"SetDisplayNameTestMiddleware");
            testContext.DisplayName = "TestDisplayName";
            await next ();
        }
    }
}