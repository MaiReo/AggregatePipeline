using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregatePipeline.Direct {
    public static class ServiceProviderExtensions {
        public static T GetService<T> (this IServiceProvider services) => (T) services.GetService (typeof (T));
        public static IEnumerable<T> GetAllServices<T> (this IServiceProvider services) => ((IEnumerable<T>) services.GetService (typeof (IEnumerable<T>))) ?? Enumerable.Empty<T> ();
    }
}