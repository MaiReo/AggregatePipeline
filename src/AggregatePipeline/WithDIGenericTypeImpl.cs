using System;
using System.Threading.Tasks;
using AggregatePipeline.Abstractions;
using Autofac;

namespace AggregatePipeline
{
    public class WithDIGenericTypeImpl : IDemoRunner
    {
        public async Task RunAsync(Func<Task> next)
        {
            var testContext = new TestContextBase();
            Console.WriteLine("With dependency injection framework and generic type implementation:");
            var iocManager = new IocManager();
            iocManager.Builder
                .RegisterGeneric(typeof(AsyncPipelineAggregator<>))
                .AsImplementedInterfaces()
                .InstancePerDependency();
            iocManager.RegisterByConvention(typeof(Program).Assembly);

            IIocResolver resolver = iocManager;

            var aggregator = resolver.Resolve<IAsyncPipelineAggregator<TestContextBase>>();

            await aggregator.AggregateAsync(testContext, next);
        }
    }
}