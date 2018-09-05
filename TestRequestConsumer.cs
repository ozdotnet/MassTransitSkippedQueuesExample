using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitSkippedQueuesExample
{
    public class TestRequestConsumer : IConsumer<TestRequest>
    {
        public Task Consume(ConsumeContext<TestRequest> context)
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));

            return context.RespondAsync(new TestResponse { Content = "Response from Consume method" });
        }
    }
}
