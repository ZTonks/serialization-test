using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace serialization_test
{
    public class TimerFunction
    {
        [Function(nameof(TimerFunction))]
        public async Task Run(
            [TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            ILogger log,
            [DurableClient] DurableTaskClient durableTaskClient)
        {
            var instanceId = "BAR";
            var durableOrchestrationStatus = await durableTaskClient.GetInstanceAsync("BAR");

            switch (durableOrchestrationStatus)
            {
                case null:
                case { RuntimeStatus: OrchestrationRuntimeStatus.Completed }:
                case { RuntimeStatus: OrchestrationRuntimeStatus.Failed }:
                case { RuntimeStatus: OrchestrationRuntimeStatus.Terminated }:
                    await durableTaskClient.ScheduleNewOrchestrationInstanceAsync(
                        nameof(MyOrchestration.DoSomething),
                        new StartOrchestrationOptions(instanceId));
                    break;
                case { RuntimeStatus: OrchestrationRuntimeStatus.Suspended }:
                    await durableTaskClient.ResumeInstanceAsync(instanceId);
                    break;
                case { RuntimeStatus: OrchestrationRuntimeStatus.Pending }:
                case { RuntimeStatus: OrchestrationRuntimeStatus.Running }:
                    break;
                default:
                    throw new Exception();
            }

            await durableTaskClient.RaiseEventAsync(
                instanceId,
                "FOO",
                eventPayload: new MyObject()
                {
                    Bar = 4,
                    Foo = "HELLO",
                });
        }
    }
}
