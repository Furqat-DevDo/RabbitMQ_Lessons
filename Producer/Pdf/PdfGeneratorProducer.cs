using System.Text;
using RabbitMQ.Client;

namespace Producer.Pdf;

public class PdfGeneratorProducer
{
    private static IChannel? _channel;
    private static IConnection? _connection;
    private PdfGeneratorProducer() {}
    public static PdfGeneratorProducer CreateInstance()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var instance = new PdfGeneratorProducer();

        _connection = factory.CreateConnection();
        _channel = _connection.CreateChannel();
        InitializeQueue();
        
        return instance;
    }
    private static  void InitializeQueue()
    {
        _channel?.QueueDeclare(
            queue: "pdf_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
    
    public async Task SendPdfRequestToQueue(string html)
    {
        var body = Encoding.UTF8.GetBytes(html);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        if (_channel is null)
            throw new ArgumentNullException(nameof(_channel));
        
        await _channel.BasicPublishAsync(exchange: string.Empty,
                routingKey: "pdf_queue",
                basicProperties: properties,
                body: body);

        Console.WriteLine(" [x] Sent html.");
    }
}