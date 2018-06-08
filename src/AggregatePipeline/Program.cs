using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AggregatePipeline {
    class Program {
        static async Task Main (string[] args) {
            var provider = new TestServiceProvider ();
            var testContext = new TestContext (provider);
            Task taskFactory () {
                Console.WriteLine ($"taskFactory");
                return Task.CompletedTask;
            };
            provider.AddTransient<ITestMiddleware, SetNameTestMiddleware> ();
            provider.AddTransient<ITestMiddleware, SetDisplayNameTestMiddleware> ();
            provider.AddTransient<ContextMiddlewareFactory, ContextMiddlewareFactory> ();
            var factory = provider.GetService<ContextMiddlewareFactory> ();
            await factory.UseMiddlewareAsync (testContext, taskFactory);
        }
    }

    public static class ServiceProviderExtensions {
        public static T GetService<T> (this IServiceProvider services) => (T) services.GetService (typeof (T));
    }

    public class TestContext {

        public TestContext (IServiceProvider serviceProvider) => this.Services = serviceProvider;
        public IServiceProvider Services { get; }

        public string Name { get; set; }

        public string DisplayName { get; set; }
    }

    public class TestServiceProvider : IServiceProvider {
        private ConcurrentDictionary<Type, ConcurrentBag<Type>> _registedServices;

        public TestServiceProvider () {
            _registedServices = new ConcurrentDictionary<Type, ConcurrentBag<Type>> ();
        }

        public void AddTransient<TService, TImplement> () where TImplement : class, TService {
            var services = _registedServices.GetOrAdd (typeof (TService), new ConcurrentBag<Type> ());
            if (services.Any (service => service == typeof (TImplement))) throw new InvalidOperationException ("Service has been registered");
            services.Add (typeof (TImplement));
        }

        public object GetService (Type serviceType) {
            if (serviceType == default) throw new ArgumentNullException (nameof (serviceType));
            object service = null;
            if (typeof (IEnumerable).IsAssignableFrom (serviceType)) {
                if (serviceType.IsGenericType && typeof (IEnumerable<>).IsAssignableFrom (serviceType.GetGenericTypeDefinition ())) {
                    var elementType = serviceType.GenericTypeArguments.First ();
                    if (!_registedServices.TryGetValue (elementType, out var services)) return null;
                    if (!services.Any ()) return null;
                    var instances = services.Select (type => Activator.CreateInstance (type)).ToArray ();
                    var array = Array.CreateInstance (elementType, instances.Length);
                    instances.CopyTo (array, 0);
                    service = array;
                }
            }
            if (service == null) {
                if (!_registedServices.TryGetValue (serviceType, out var services)) return null;
                if (!services.Any ()) return null;
                var implementationType = services.LastOrDefault ();
                if (implementationType == null) return null;
                service = Activator.CreateInstance (serviceType);
            }
            return service;
        }
    }

    public interface ITestMiddleware {
        Task HandleAsync (TestContext testContext, Func<Task> next);
    }

    public class SetNameTestMiddleware : ITestMiddleware {
        public async Task HandleAsync (TestContext testContext, Func<Task> next) {
            Console.WriteLine ($"SetNameTestMiddleware");
            testContext.Name = "TestName";
            await next ();
        }
    }

    public class SetDisplayNameTestMiddleware : ITestMiddleware {
        public async Task HandleAsync (TestContext testContext, Func<Task> next) {
            Console.WriteLine ($"SetDisplayNameTestMiddleware");
            testContext.DisplayName = "TestDisplayName";
            await next ();
        }
    }

    public class ContextMiddlewareFactory {

        public async Task UseMiddlewareAsync (TestContext context, Func<Task> next) {
            var testMiddlewares = (IEnumerable<ITestMiddleware>) context.Services.GetService (typeof (IEnumerable<ITestMiddleware>));
            if (testMiddlewares == null) {
                await next ();
                return;
            }
            var aggregater = new TestMiddlewareAggregater (testMiddlewares, next);
            await aggregater.AggregateAsync (context);
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