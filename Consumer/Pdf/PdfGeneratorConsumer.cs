using System.Text;
using Contracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Pdf;

public class PdfGeneratorConsumer : IDisposable
{
    private readonly IChannel _channel;
    private readonly IConnection _connection;

    public PdfGenerator PdfGeneratorManager { get; private set; } = null!;

    public PdfGeneratorConsumer()
    {
        CreatePdfGenerator();

        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateChannel();

        InitializeQueue();

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += ConsumePdfGeneratorRequest!;

        _channel.BasicConsume(queue: "pdf_queue", autoAck: false, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    private void CreatePdfGenerator()
    {
        var factory = new LoggerFactory();
        var logger = factory.CreateLogger<PdfGenerator>();
        PdfGeneratorManager = new PdfGenerator(logger);
    }

    private void InitializeQueue()
    {
        _channel.QueueDeclare(queue: "pdf_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    private async void ConsumePdfGeneratorRequest(object model, BasicDeliverEventArgs ea)
    {
        try
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await GeneratePdfFromQueue(message);
            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during PDF generation: {ex.Message}");
        }
    }

    private async Task GeneratePdfFromQueue(string message)
    {
        await using (Stream pdfStream = await PdfGeneratorManager.GeneratePdfAsync(message))
        {
            string folderPath = "./app";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"GeneratedPdf_{Guid.NewGuid()}.pdf";

            string filePath = Path.Combine(folderPath, fileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await pdfStream.CopyToAsync(fileStream);
            }
        }

        Console.WriteLine(" [x] Done");
    }
}
