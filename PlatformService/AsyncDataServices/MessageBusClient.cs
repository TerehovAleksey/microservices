using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        var configuration1 = configuration;

        var factory = new ConnectionFactory()
        {
            HostName = configuration1["RabbitMQHost"],
            Port = int.Parse(configuration1["RabbitMQPort"])
        };
        
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            Console.Write("--> Connected to message bus");
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not connect to the message bys: {e.Message}");
            throw;
        }
    }

    public void PublishNewPlatform(PlatformPublishedDto dto)
    {
        var message = JsonSerializer.Serialize(dto);
        if (_connection.IsOpen)
        {
            Console.Write("--> RabbitMQ connection Open, sending message");
            SendMessage(message);
        }
        else
        {
            Console.Write("--> RabbitMQ connection closed, not sending");
        }
    }

    public void Dispose()
    {
        Console.Write("--> MessageBus disposed");
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
            _connection.ConnectionShutdown -= RabbitMQ_ConnectionShutdown;
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
        
        Console.WriteLine($"--> We have sent {message}");
    }
    
    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.Write("--> RabbitMQ connection shutdown");
    }
}