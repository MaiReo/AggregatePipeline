using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AggregatePipeline.Direct {
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
}