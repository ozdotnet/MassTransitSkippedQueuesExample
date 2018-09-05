using System;
using MassTransit;

namespace MassTransitSkippedQueuesExample
{
    class Program
    {
        private static IBusControl _mtBus;

        static void Main()
        {
            try
            {
                CreateMtBus();
                var requestClient = _mtBus.CreatePublishRequestClient<TestRequest, TestResponse>(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                var response = requestClient.Request(new TestRequest());
                string contentStr = response.Result.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }

            //_mtBus.Stop();
        }

        public static void CreateMtBus()
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
    }
}
