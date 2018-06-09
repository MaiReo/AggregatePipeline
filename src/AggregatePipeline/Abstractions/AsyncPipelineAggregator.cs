using System;
using System.Threading.Tasks;

namespace AggregatePipeline
{

    public class AsyncPipelineAggregator<T> : IAsyncPipelineAggregator<T> {
        private readonly IIocResolver iocResolver;

        public AsyncPipelineAggregator (IIocResolver iocResolver) {
            this.iocResolver = iocResolver;
        }
        public async Task AggregateAsync (T context, Func<Task> next) {
            var handlers = iocResolver.ResolveAll<IAsyncPipelineContextHandler<T>> ();
            var aggregationContext = AsyncPipelineAggregationContext.Build(handlers, next);
            await aggregationContext.AggregateAsync(context);
        }

    }
}