# LionWeb notification system API

This document explains LionWeb notification system API through use cases. 

## Use cases
### How to get informed about changes
Node API exposes `GetNotificationHandler()`. It provides and raises notifications about nodes and their features.
`INotificationHandler.Connect` specifies/connects a receiver to receive notifications sent by a sender. Receiver filters notifications via its `Handles` member method and takes necessary actions for the received notification type.     

Code below demonstrates API usage demonstrating how to get informed changes from a partition.
```csharp
var node = new Geometry("partition");
        
var sender = node.GetNotificationHandler();
var receiver = new Observer();
INotificationHandler.Connect(from: sender, to: receiver);

node.Documentation = new Documentation("added");
```
Code below demonstrates API usage demonstrating how to get informed changes from a forest.
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
A composite notification consists of other forest and/or partition notifications.


### How to replicate changes