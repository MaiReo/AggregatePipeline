using System;
using System.Threading.Tasks;

namespace AggregatePipeline
{
    public interface IDemoRunner
    {
        Task RunAsync(Func<Task> next);
    }
}