using System;
using System.Threading.Tasks;
using AggregatePipeline.DependencyInjection;

namespace AggregatePipeline.Direct {
    public class SetDisplayNameTestMiddleware : ITestMiddleware, ILifeStyleTransient {
        public async Task HandleAsync (TestContext testContext, Func<Task> next) {
            Console.WriteLine ($"SetDisplayNameTestMiddleware");
            testContext.DisplayName = "TestDisplayName";
            await next ();
        }
    }
}