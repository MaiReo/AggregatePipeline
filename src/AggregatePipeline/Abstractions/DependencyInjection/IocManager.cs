using System.Collections.Generic;
using System.Reflection;
using AggregatePipeline.DependencyInjection;
using Autofac;

namespace AggregatePipeline {
    public class IocManager : IIocManager, IIocResolver {

        public static IocManager Instance { get; private set; }
        public ContainerBuilder Builder { get; }
        public IContainer Container { get; private set; }

        static IocManager () {
            Instance = new IocManager ();
        }

        public IocManager () {
            Builder = new ContainerBuilder ();
            Builder.RegisterInstance (this)
                .AsSelf ()
                .AsImplementedInterfaces ()
                .SingleInstance ();
            Instance = this;
        }

        private void BuildIfNot () {
            Container = Container ?? Builder.Build ();
        }

        private bool CanRegister => Container == null;
        public void Register<TService, TImplement> (LifeStyle lifestyle = LifeStyle.Singleton) where TImplement : class, TService {
            if (!CanRegister)
                throw new System.InvalidOperationException ();
            var registration = Builder.RegisterType<TImplement> ()
                .AsSelf ()
                .As<TService> ()
                .PropertiesAutowired ();
            switch (lifestyle) {
                case LifeStyle.Transient:
                    registration = registration.InstancePerDependency ();
                    break;
                default:
                    registration = registration.SingleInstance ();
                    break;
            }
        }

        public T Resolve<T> () {
            BuildIfNot ();
            return Container.Resolve<T> ();
        }
        public IEnumerable<T> ResolveAll<T> () {
            BuildIfNot ();
            return Container.Resolve<IEnumerable<T>> ();
        }

        public void RegisterByConvention (Assembly assembly) {
            if (!CanRegister)
                throw new System.InvalidOperationException ();
            Builder.RegisterAssemblyTypes (assembly)
                .AssignableTo<ILifeStyleSingleton> ()
                .AsSelf ()
                .AsImplementedInterfaces ()
                .PropertiesAutowired ()
                .SingleInstance ();

            Builder.RegisterAssemblyTypes (assembly)
                .AssignableTo<ILifeStyleTransient> ()
                .AsSelf ()
                .AsImplementedInterfaces ()
                .PropertiesAutowired ()
                .InstancePerDependency ();
        }
    }
}