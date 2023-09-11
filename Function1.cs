using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace serialization_test
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task Run(
            [TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            ILogger log,
            [DurableClient] IDurableOrchestrationClient durableTaskClient)
        {
            var instanceId = "BAR";
            var durableOrchestrationStatus = await durableTaskClient.GetStatusAsync("BAR");

            switch (durableOrchestrationStatus)
            {
                case null:
                case { RuntimeStatus: OrchestrationRuntimeStatus.Completed }:
                case { RuntimeStatus: OrchestrationRuntimeStatus.Failed }:
                case { RuntimeStatus: OrchestrationRuntimeStatus.Terminated }:
                    await durableTaskClient.StartNewAsync(
                        nameof(MyOchrestration.DoSomething),
                        instanceId);
                    break;
                case { RuntimeStatus: OrchestrationRuntimeStatus.Suspended }:
                    await durableTaskClient.ResumeAsync(instanceId, "lol");
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
                eventData: new MyObject() {  Bar = 4, Foo = "HELLO" });
        }
    }
}
