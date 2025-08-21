# LionWeb notification system API

This document explains the LionWeb notification system API through some use cases. 

## Use cases
### How to get informed about changes
Every partition node of a model, which support notification API, exposes `GetNotificationHandler()`. 
A notification handler initiates and raises notifications about nodes and their features when there is a change to the model.
Each change to the model raises a notification.  
`INotificationHandler.Connect` specifies a receiver of notifications sent by a sender. In the example below,  `Observer` is a receiver which counts and 
prints out the received notifications in its `Receive` member method.  Receiver can filter notifications via its `Handles` member method.     

Code below gives an example of API usage demonstrating how to get informed about changes to a partition.
```csharp
var node = new Geometry("partition");
        
var sender = node.GetNotificationHandler();
var receiver = new Observer();
if (sender != null)
{
    INotificationHandler.Connect(from: sender, to: receiver);
}

// This is a change to the model
node.Documentation = new Documentation("added");
```
Code below gives an example of API usage demonstrating how to get informed about changes to a forest.
```csharp
var node = new Geometry("partition");
var forest = new Forest();

var sender = forest.GetNotificationHandler();
var receiver = new Observer();
INotificationHandler.Connect(from: sender, to: receiver);

// This is a change to the forest
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

### How to collect multiple changes into one change set 
Notifications raised by multiple changes to a model can be collected into one change set. 
A composite notification composes other forest and/or partition notifications into one
composite notification. Follow the comments below in the code block for further explanation.

```csharp
var partition = new Geometry("partition");

var sender = partition.GetNotificationHandler();

// NotificationCompositor implements composite notification logic. 
// It can receive and send notifications. 
var compositor = new NotificationCompositor("compositor");
if (sender != null)
{
    INotificationHandler.Connect(from: sender, to: compositor);
}

// PartitionEventCounter (see the class definition below)
// is a receiving notification handler and counts received notifications.
var counter = new PartitionEventCounter();
INotificationHandler.Connect(from: compositor, to: counter);

// Creates a new composite notification to collect incoming notifications
var composite = compositor.Push(); 

// The notifications raised by three changes below will be added to the created composite notification.
partition.Documentation = new Documentation("documentation");
partition.Documentation.Text = "hello";
partition.AddShapes([new Circle("c")]);

Console.WriteLine($"Size of composite: {composite.Parts.Count}");  // Size of composite: 3 (composite consists of 3 notifications)
Console.WriteLine($"Counter: {counter.Count}"); // Counter: 0 (nothing is sent from compositor to counter yet)

compositor.Pop(true); // pops and sends the composite notification on top the stack

Console.WriteLine($"Counter: {counter.Count}"); // Counter: 1 (counter receives 1 composite notification)
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

Partition replicator:

```csharp
var circle = new Circle("c");
var node = new Geometry("partition") { Shapes = [circle] };

var clone = Clone(node);

var cloneHandler = new NodeCloneNotificationHandler(node.GetId());
INotificationHandler.Connect(node.GetNotificationHandler(), cloneHandler);

var replicator = PartitionReplicator.Create(clone, new SharedNodeMap(), node.GetId());
INotificationHandler.Connect(cloneHandler, replicator);

circle.Name = "Hello";

AssertEquals([node], [clone]);
```

Forest replicator:

```csharp
var moved = new Documentation("moved");
var originPartition = new Geometry("originPartition") { Shapes = [new Line("l") { ShapeDocs = moved }] };

var node = new Geometry("a") { };

var originalForest = new Forest();
var cloneForest = new Forest();

var cloneHandler = new NodeCloneNotificationHandler("forestCloner");
INotificationHandler.Connect(originalForest.GetNotificationHandler(), cloneHandler);

var replicator = ForestReplicator.Create(cloneForest, new SharedNodeMap(), null);
INotificationHandler.Connect(cloneHandler, replicator);

var receiver = new TestLocalForestChangeNotificationHandler(originalForest, cloneHandler);
INotificationHandler.Connect(originalForest.GetNotificationHandler(), receiver);

originalForest.AddPartitions([node, originPartition]);

node.Documentation = moved;

AssertEquals([node, originPartition], cloneForest.Partitions.OrderBy(p => p.GetId()).ToList());
```
