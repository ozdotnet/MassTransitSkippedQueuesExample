using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes.Util;
using MassTransit;

namespace MassTransitSkippedQueuesExample
{
    class Program
    {
        private static IBusControl _mtBus;

        static void Main(string[] args)
        {
            CreateMtBus();

            var requestClient = _mtBus.CreatePublishRequestClient<TestRequest, TestResponse>(TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(5));

            var response = requestClient.Request(new TestRequest());

            string contentStr;

            try
            {
                contentStr = response.Result.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }

            //_mtBus.Stop();
        }

        public static void CreateMtBus()
        {
            try
            {
                _mtBus = Bus.Factory.CreateUsingRabbitMq(busFactoryConfig =>
                {
                    var host = busFactoryConfig.Host(new Uri("rabbitmq://localhost/"), hostConfig =>
                    {
                        hostConfig.Username("guest");
                        hostConfig.Password("guest");
                    });

                    busFactoryConfig.ReceiveEndpoint(host, receiveEndpointConfig =>
                    {
                        receiveEndpointConfig.Consumer<TestRequestConsumer>();
                    });
                });

                _mtBus.Start();

                Console.WriteLine("Publish Bus created.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error. {e}");
            }
        }
    }

    public class TestRequest
    {

    }

    public class TestResponse
    {
        public string Content { get; set; }
    }

    public class TestRequestConsumer : IConsumer<TestRequest>
    {
        public Task Consume(ConsumeContext<TestRequest> context)
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));

            return context.RespondAsync(new TestResponse {Content = "Response from Consume method"});
        }
    }
}
