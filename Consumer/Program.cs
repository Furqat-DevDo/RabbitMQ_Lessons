using Consumer;

var consumer = new UserCreatedConsumer();
await consumer.ConsumeCreatedUser();
