using System;
using System.Threading.Tasks;

namespace AggregatePipeline
{
    public interface IAsyncPipelineContextHandler<T> {
        Task HandleAsync (T context, Func<Task> next);
    }
}