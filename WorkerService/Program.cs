using ChildWorkerService.Messages;

const string EndpointName = "NsbActivities.WorkerService";

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var endpointConfiguration = new EndpointConfiguration(EndpointName);

endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

var transport = new RabbitMQTransport(
    RoutingTopology.Conventional(QueueType.Classic),
    builder.Configuration.GetConnectionString("broker")
);
var transportSettings = endpointConfiguration.UseTransport(transport);

transportSettings.RouteToEndpoint(typeof(MakeItYell).Assembly, "NsbActivities.ChildWorkerService");

endpointConfiguration.UsePersistence<LearningPersistence>();

endpointConfiguration.EnableInstallers();

endpointConfiguration.EnableOpenTelemetry();

builder.UseNServiceBus(endpointConfiguration);

builder.Services.AddHttpClient("web", client => client.BaseAddress = new Uri("https://web"));

var host = builder.Build();

host.Run();
