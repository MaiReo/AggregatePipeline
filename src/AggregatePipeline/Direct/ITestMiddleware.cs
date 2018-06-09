using System;
using System.Threading.Tasks;

namespace AggregatePipeline.Direct {
    public interface ITestMiddleware  {
        Task HandleAsync (TestContext testContext, Func<Task> next);
    }
}