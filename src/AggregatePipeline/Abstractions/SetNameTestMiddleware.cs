using System;
using System.Threading.Tasks;
using AggregatePipeline.DependencyInjection;

namespace AggregatePipeline.Abstractions {
    public class SetNameTestMiddleware : IAsyncPipelineContextHandler<TestContextBase>, ILifeStyleTransient {
        public async Task HandleAsync (TestContextBase testContext, Func<Task> next) {
            Console.WriteLine ($"SetNameTestMiddlewareBase");
            testContext.Name = "TestName";
            await next ();
        }

    }
}