using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AggregatePipeline.Direct {
    public class ContextMiddlewareFactory {
        public async Task UseMiddlewareAsync (TestContext context, Func<Task> next) {
            var testMiddlewares = context.Services.GetAllServices<ITestMiddleware> ();
            var aggregater = new TestMiddlewareAggregater (testMiddlewares, next);
            await aggregater.AggregateAsync (context);
        }

        public class AsyncPipelineAggregator {

        }

        private class TestMiddlewareAggregater {
            private readonly IEnumerator<ITestMiddleware> _enumerator;
            private readonly Func<Task> _next;

            public TestMiddlewareAggregater (IEnumerable<ITestMiddleware> middlewares, Func<Task> next) {
                _enumerator = middlewares.GetEnumerator ();
                _next = next;
            }

            public async Task AggregateAsync (TestContext context) {
                if (!_enumerator.MoveNext ()) {
                    await _next ();
                    return;
                }
                await _enumerator.Current.HandleAsync (context, async () => await AggregateAsync (context));
            }
        }
    }
}