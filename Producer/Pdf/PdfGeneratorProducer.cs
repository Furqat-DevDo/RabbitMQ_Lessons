using System.Text;
using RabbitMQ.Client;

namespace Producer.Pdf;

public class PdfGeneratorProducer
{
    public async Task GeneratePdfRequest(string html)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await  connection.CreateChannelAsync();

        channel.QueueDeclare(queue: "pdf_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        var body = Encoding.UTF8.GetBytes(html);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: "pdf_queue",
            basicProperties: properties,
            body: body);
        
        Console.WriteLine(" [x] Sent html.");
    }
}