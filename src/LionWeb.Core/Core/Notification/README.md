# LionWeb notification system API

This document explains LionWeb notification system API through some use cases. 

## Use cases
### How to get informed about changes
Node API exposes `GetNotificationHandler()`. It provides and raises notifications about nodes and their features.
`INotificationHandler.Connect` specifies/connects a receiver to receive notifications sent by a sender. 
Receiver filters notifications via its `Handles` member method and takes necessary actions for the received notification type.     

Code below gives an example of API usage demonstrating how to get informed changes from a partition.
```csharp
var node = new Geometry("partition");
        
var sender = node.GetNotificationHandler();
var receiver = new Observer();
INotificationHandler.Connect(from: sender, to: receiver);

node.Documentation = new Documentation("added");
```
Code below gives an example of API usage demonstrating how to get informed changes from a forest.
```csharp
var node = new Geometry("partition");
var forest = new Forest();

var sender = forest.GetNotificationHandler();
var receiver = new Observer();
INotificationHandler.Connect(from: sender, to: receiver);

forest.AddPartitions([node]); 
```

```csharp
public class Observer: IReceivingNotificationHandler
{
    public int NotificationCount { get; private set; }
    
    public void Dispose() => throw new NotImplementedException();

    // Handles all type of notifications
    public bool Handles(params Type[] notificationTypes) => true;

    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        // Take necessary action for the received notification 
        NotificationCount++;
        Console.WriteLine(notification);
    }
}
```

### How to compose several notifications into one composite notification
A composite notification composes other forest and/or partition notifications into one
composite notification. Follow the comments below in the code block for further explanation.

```csharp
var partition = new Geometry("partition");

// NotificationCompositor is a type of handler that implements compositor logic.
// It can receive and send notifications.
var compositor = new NotificationCompositor("compositor");
INotificationHandler.Connect(partition.GetNotificationHandler(), compositor);

// PartitionEventCounter (see the class definition below)
// is a receiving notification handler and counts received notifications.
var counter = new PartitionEventCounter(); 
INotificationHandler.Connect(compositor, counter);

// Create a new composite notification
var composite = compositor.Push(); 

// The following three notifications will be added to the created composite notifications.
partition.Documentation = new Documentation("documentation");
partition.Documentation.Text = "hello";
partition.AddShapes([new Circle("c")]);

Assert.AreEqual(3, composite.Parts.Count); // composite consists of 3 notifications
Assert.AreEqual(0, counter.Count); // count is 0; nothing is sent from compositor to counter yet 

compositor.Pop(true); // pops and sends the composite on top the the stack
Assert.AreEqual(1, counter.Count); // counter receives 1 composite notificaion
```

```csharp
internal class PartitionEventCounter() : NotificationHandlerBase(null), IReceivingNotificationHandler
{
    public int Count { get; private set; }
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification) =>
    Count++;
}
```
### How to replicate changes