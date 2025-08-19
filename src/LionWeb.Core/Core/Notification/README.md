# LionWeb notification system API

This document explains LionWeb notification system API through use cases. 

## Use cases
### How to get informed about changes
Node API exposes `GetNotificationHandler()`. It provides and raises notifications about nodes and their features.
`INotificationHandler.Connect` specifies/connects a receiver to receive notifications sent by a sender. Receiver filters notifications via its `Handles` member method and takes necessary actions for the received notification type.     

```csharp
var node = new Geometry("a");
        
var sender = node.GetNotificationHandler();
var receiver = new Observer();
INotificationHandler.Connect(from: sender, to: receiver);

node.Documentation = new Documentation("added");
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
### How to replicate changes


### How to composite changes 
Collecting notifications 
