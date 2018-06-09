using System;
using System.Threading.Tasks;

namespace AggregatePipeline
{
    public interface IAsyncPipelineAggregator<T> {
        Task AggregateAsync (T context, Func<Task> next);
    }
}