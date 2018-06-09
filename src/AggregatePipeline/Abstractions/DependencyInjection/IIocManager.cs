using System.Reflection;
using AggregatePipeline.DependencyInjection;

namespace AggregatePipeline {
    public interface IIocManager {
        void Register<TService, TImplement> (LifeStyle lifestyle = LifeStyle.Singleton) where TImplement : class, TService;
        void RegisterByConvention (Assembly assembly);
    }
}