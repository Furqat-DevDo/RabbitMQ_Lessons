using Consumer;
using Consumer.Pdf;
using Contracts;
using Microsoft.Extensions.Logging;

// var consumer = new UserCreatedConsumer();
// await consumer.ConsumeCreatedUser();

var consumer = new PdfGeneratorConsumer();

Console.ReadKey();