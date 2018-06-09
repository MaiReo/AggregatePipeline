using System;
using System.Threading.Tasks;
using AggregatePipeline.Direct;

namespace AggregatePipeline
{
    public class DirectImpl : IDemoRunner
    {
        public async Task RunAsync(Func<Task> next)
        {
             var provider = new TestServiceProvider ();
             var testContext = new TestContext (provider);
            provider.AddTransient<ITestMiddleware, SetNameTestMiddleware> ();
            provider.AddTransient<ITestMiddleware, SetDisplayNameTestMiddleware> ();
            provider.AddTransient<ContextMiddlewareFactory, ContextMiddlewareFactory> ();
            var factory = provider.GetService<ContextMiddlewareFactory> ();
            Console.WriteLine("Without generic type implementation:");
            await factory.UseMiddlewareAsync (testContext, next);
        }
    }
}