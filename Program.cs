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


