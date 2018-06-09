using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AggregatePipeline {
    class Program {
        static async Task Main (string[] args) {
            var directRunner = new DirectImpl ();
            var withDIRunner = new WithDIGenericTypeImpl ();
            Task next () {
                Console.WriteLine ("next");
                return Task.CompletedTask;
            }
            await directRunner.RunAsync (next);
            await withDIRunner.RunAsync (next);
        }
    }
}