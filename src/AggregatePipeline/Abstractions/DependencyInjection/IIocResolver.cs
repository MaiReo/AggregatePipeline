using System.Collections.Generic;

namespace AggregatePipeline {
    public interface IIocResolver {
        T Resolve<T>();
        IEnumerable<T> ResolveAll<T> ();
    }
}