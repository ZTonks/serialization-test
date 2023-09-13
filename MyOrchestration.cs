using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using System;
using System.Threading.Tasks;

namespace serialization_test
{
    public class MyOrchestration
    {
        public MyOrchestration()
        {
        }

        [Function(nameof(DoSomething))]
        public async Task DoSomething(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var foo = await context.WaitForExternalEvent<MyObject>("FOO");

            await context.CreateTimer(
                context.CurrentUtcDateTime.Add(TimeSpan.FromSeconds(15)),
                cancellationToken: default);

            context.ContinueAsNew(preserveUnprocessedEvents: true);
        }
    }
}
