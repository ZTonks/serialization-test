using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace serialization_test
{
    public class MyOrchestration
    {
        public MyOrchestration()
        {
        }

        [FunctionName(nameof(DoSomething))]
        public async Task DoSomething(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            await context.WaitForExternalEvent<MyObject>("FOO");

            await context.CreateTimer(
                context.CurrentUtcDateTime.Add(TimeSpan.FromSeconds(15)),
                cancelToken: default);

            context.ContinueAsNew(null, preserveUnprocessedEvents: true);
        }
    }
}
