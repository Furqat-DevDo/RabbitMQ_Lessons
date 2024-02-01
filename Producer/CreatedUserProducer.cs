using System.Text;
using System.Text.Json;
using Contracts;
using RabbitMQ.Client;

namespace Producer;

public class CreatedUserProducer
{
    public async Task PublishCreatedUser(CreatedUserMessage userMessage)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        
        channel.QueueDeclare(queue: "users",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);
        
        var body = JsonSerializer.SerializeToUtf8Bytes(userMessage);
    
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "users",
            body: body,
            mandatory: false);
        
        Console.WriteLine($"Message Published : {userMessage}");
    }
    
}