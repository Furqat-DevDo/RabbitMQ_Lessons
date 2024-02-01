using Contracts;
using Contracts.Users;
using Microsoft.Extensions.Logging;
using Producer;
using Producer.Pdf;

// int id = 1;
//
// while (true)
// {
//    var producer = new CreatedUserProducer();
//    var user = new CreatedUserMessage(id,"Furqat","Furqat@gamil.com",new DocumentInfo(id,DateTime.UtcNow,"Ad2059384"));
//    await Task.Delay(TimeSpan.FromSeconds(5));
//    await producer.PublishCreatedUser(user);
//    id++;
//    
//    if (id == 100) break;
// }

var producer = new PdfGeneratorProducer();

while (true)
{
    var html = File.ReadAllText(@"C:\Users\furqa\Pub-Sub\Contracts\Invoice.html");
    await producer.GeneratePdfRequest(html);
}

