using System;
using AggregatePipeline.Abstractions;

namespace AggregatePipeline.Direct {
    public class TestContext : TestContextBase {
        public TestContext (IServiceProvider serviceProvider) => this.Services = serviceProvider;
        public virtual IServiceProvider Services { get; }

    }
}