using System;
using System.Threading.Tasks;
using AggregatePipeline.DependencyInjection;

namespace AggregatePipeline.Direct {
    public class SetNameTestMiddleware : ITestMiddleware, ILifeStyleTransient {
        public async Task HandleAsync (TestContext testContext, Func<Task> next) {
            Console.WriteLine ($"SetNameTestMiddleware");
            testContext.Name = "TestName";
            await next ();
        }

    }
}