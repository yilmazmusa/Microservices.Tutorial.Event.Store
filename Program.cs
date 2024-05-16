#region İnceleme

//using EventStore.Client;
//using System.Text.Json;

//string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false";
//var settings = EventStoreClientSettings.Create(connectionString); //Erişim ayarlarını yaptık
//var client = new EventStoreClient(settings); //client üzerinden erişimi sağladık.



//OrderPlaceEvent orderPlaceEvent = new()
//{
//    OrderId = 1,
//    TotalAmount = 100
//};

////while (true)
////{
////    EventData eventData = new(

////    eventId: Uuid.NewUuid(),
////    type: orderPlaceEvent.GetType().Name,
////    data: JsonSerializer.SerializeToUtf8Bytes(orderPlaceEvent)
////    ); // orderPlaceEvent'i EventData türünden bir evente dönüştürdük.


////    //EventData türüne dönüştürdüğümüz eventi yayınlayacağız.

////    await client.AppendToStreamAsync(
////        streamName: "order-stream",
////        expectedState: StreamState.Any,
////        eventData: new[] { eventData }
////        );


////}

////Stream(Event Store kaydedilmiş) dataları Okuma

//var events = client.ReadStreamAsync(
//    streamName: "order-stream", //ismi order-stream olan eventin datalarını getirsin
//    direction: Direction.Forwards, //Baştan sonra sıralasın
//    revision: StreamPosition.Start // Stream'ın pozisyonunu belirtiyoruz en baştan başlasın getirsin diyoruz
//    );

//var datas = await events.ToListAsync();
//Console.WriteLine();


//await client.SubscribeToStreamAsync(
//    streamName: "order-stream", //order-stream a subscribe(abone) olduk
//    start: FromStream.Start,
//    eventAppeared: async (streamsubscription, resolvedEvent, cancellationToken) => //resolvedEvent parametresi bu subscribe'ı tetikleyecek olan eventi verecek
//    {
//        OrderPlaceEvent @event = JsonSerializer.Deserialize<OrderPlaceEvent>(resolvedEvent.Event.Data.ToArray());

//        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(@event));
//    },
//    subscriptionDropped: (streamSubcription, subscriptionDroppedReason, exception) => Console.WriteLine("Disconnected") // subscriptionDropped bağlantı kopukluğu olduğu zaman tetiklenecek yer
//    );

//Console.Read();




//class OrderPlaceEvent
//{
//    public int OrderId { get; set; }
//    public double TotalAmount { get; set; }
//}


#endregion


#region BAKIYE ORNEK


using EventStore.Client;
using System.Text.Json;
using System.Threading;


EventStoreService eventStoreService = new();

//Eventler
AccountCreatedEvent accountCreatedEvent = new()
{
    AccountId = "12345",
    CostumerId = "98765",
    StartBalance = 0,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent1 = new()
{
    AccountId = "12345",
    Amount = 1000,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent2 = new()
{
    AccountId = "12345",
    Amount = 500,
    Date = DateTime.UtcNow.Date
};
MoneyWithdrawnEvent moneyWithdrawnEvent = new()
{
    AccountId = "12345",
    Amount = 200,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent3 = new()
{
    AccountId = "12345",
    Amount = 50,
    Date = DateTime.UtcNow.Date
};
MoneyTransferredEvent moneyTransferredEvent1 = new()
{
    AccountId = "12345",
    Amount = 250,
    Date = DateTime.UtcNow.Date
};
MoneyTransferredEvent moneyTransferredEvent2 = new()
{
    AccountId = "12345",
    Amount = 150,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent4 = new()
{
    AccountId = "12345",
    Amount = 2000,
    Date = DateTime.UtcNow.Date
};

//Eventleri gönderiyoruz.
//await eventStoreService.AppendToStreamAsync( 
//    streamName: $"customer-{accountCreatedEvent.CostumerId}-stream",
//    new[]
//    {
//        eventStoreService.GenerateEventData(accountCreatedEvent),
//        eventStoreService.GenerateEventData(moneyDepositedEvent1),
//        eventStoreService.GenerateEventData(moneyDepositedEvent2),
//        eventStoreService.GenerateEventData(moneyWithdrawnEvent),
//        eventStoreService.GenerateEventData(moneyDepositedEvent3),
//        eventStoreService.GenerateEventData(moneyTransferredEvent1),
//        eventStoreService.GenerateEventData(moneyTransferredEvent2),
//        eventStoreService.GenerateEventData(moneyDepositedEvent4),
//    }
//    );



BalanceInfo balanceInfo = new();

await eventStoreService.SubscribeToStreamAsync(

    streamName: $"customer-{accountCreatedEvent.CostumerId}-stream",
    async (streamSubscription, resolvedEvent, cancellationToken) =>
    {
        string eventType = resolvedEvent.Event.EventType;
        object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(), Type.GetType(eventType)); //Elimizde metinsel bir ifade varsa önce onu array'e çevirip sonra type türünden belirtebiliyoruz


        switch (@event)
        {
            case AccountCreatedEvent e:
                balanceInfo.AccountId = e.AccountId;
                balanceInfo.Balance = e.StartBalance;
                break;

            case MoneyDepositedEvent e:
                balanceInfo.Balance += e.Amount;
                break;

            case MoneyWithdrawnEvent e:
                balanceInfo.Balance -= e.Amount;
                break;

            case MoneyTransferredEvent e:
                balanceInfo.Balance -= e.Amount;
                break;

        }

        await Console.Out.WriteLineAsync("*************BALANCE***********");
        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(balanceInfo));
        await Console.Out.WriteLineAsync("*************BALANCE***********");
        await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync();
    }
    );

Console.Read();


class EventStoreService
{
    EventStoreClientSettings GetEventStoreClientSettings(string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false")
        => EventStoreClientSettings.Create(connectionString);

    EventStoreClient Client { get => new EventStoreClient(GetEventStoreClientSettings()); }

    public async Task AppendToStreamAsync(string streamName, IEnumerable<EventData> eventData)
        => await Client.AppendToStreamAsync(
            streamName: streamName,
            eventData: eventData,
            expectedState: StreamState.Any
            );


    public EventData GenerateEventData(object @event)
    => new(
        eventId: Uuid.NewUuid(),
        type: @event.GetType().Name,
        data: JsonSerializer.SerializeToUtf8Bytes(@event)
        );

    public async Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared)
        => Client.SubscribeToStreamAsync(
            streamName: streamName,
            start: FromStream.Start,
            eventAppeared: eventAppeared,
            subscriptionDropped: (x, y, z) => Console.WriteLine("Disconnected!!")
            );


}

class BalanceInfo
{
    public string AccountId { get; set; }
    public int Balance { get; set; }
}

class AccountCreatedEvent
{
    public string AccountId { get; set; }
    public string CostumerId { get; set; }
    public int StartBalance { get; set; }
    public DateTime Date { get; set; }
}

class MoneyDepositedEvent
{
    public string AccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}

class MoneyWithdrawnEvent
{
    public string AccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}

class MoneyTransferredEvent
{
    public string AccountId { get; set; }
    public string TargetAccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}

#endregion


