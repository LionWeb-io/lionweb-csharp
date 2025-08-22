# LionWeb notification system API

This document explains the LionWeb notification system API through some use cases. 

## Use cases
### How to get informed about changes
Every partition node of a model, which supports notification API, exposes `GetNotificationHandler()`. 
A notification is triggered when there is a change to the model.
`INotificationHandler.Connect` connects a receiver to a sender. In the example below, `Observer` is a receiver which counts and 
prints out the received notifications in its `Receive` member method.  Receiver can filter notifications via its `Handles` member method.     

Code below gives an example of API usage demonstrating how to get informed about changes to a partition.
```csharp
var partition = new Geometry("geo");
        
var sender = partition.GetNotificationHandler();
var receiver = new Observer(); // see Observer class definition below
if (sender != null) // if model does not support notifications, sender might be null.   
{
    INotificationHandler.Connect(from: sender, to: receiver);
}

// This is a change to the model
partition.Documentation = new Documentation("added");
```
Code below gives an example of API usage demonstrating how to get informed about changes to a forest.
```csharp
var forest = new Forest();

var sender = forest.GetNotificationHandler();
var receiver = new Observer();
INotificationHandler.Connect(from: sender, to: receiver);

var partition = new Geometry("geo");

// This is a change to the forest
forest.AddPartitions([partition]); 
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
var partition = new Geometry("geo");

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

Partition replicator replicates received notifications on a local equivalent partitions.

```csharp
var circle = new Circle("c");
var partition = new Geometry("geo") { Shapes = [circle] };

var sender = partition.GetNotificationHandler();

// This handler creates a corresponding notifications for the partition clone and raises the notifications
// (see TestNodeCloneNotificationHandler below)
var cloneHandler = new TestNodeCloneNotificationHandler(partition.GetId());
if (sender != null)
{
    INotificationHandler.Connect(from: sender, to: cloneHandler);
}

var clone = Clone(partition);
// Replicates notifications for the cloned partition. We receive a PropertyAddedNotification for the cloned partition and 
// this class adds the same property value to the cloned partition node.
var replicator = PartitionReplicator.Create(clone, new SharedNodeMap(), sender: partition.GetId());
INotificationHandler.Connect(from: cloneHandler, to: replicator);

// This change triggers PropertyAddedNotification notification
circle.Name = "Hello";
```

In the class below; for the sake of keeping the code short, `Receive` handles only three type of notifications. 
```csharp
internal class TestNodeCloneNotificationHandler(object? sender) : NotificationHandlerBase(sender),
IConnectingNotificationHandler
{
    private readonly Dictionary<IReadableNode, IReadableNode> _memoization = [];
    private readonly Dictionary<ISendingNotificationHandler, ISendingNotificationHandler> _handlerMemoization = [];

    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        INotification result = notification switch
        {
            PartitionAddedNotification e => e with { NewPartition = Clone(e.NewPartition) },
            PropertyAddedNotification e => e with { Node = Clone(e.Node), },
            ChildMovedFromOtherContainmentNotification e => e with
            {
                MovedChild = Clone(e.MovedChild), NewParent = Clone(e.NewParent), OldParent = Clone(e.OldParent)
            },
        };

        if (result is IForestNotification f)
        {
            _handlerMemoization[correspondingHandler] = f.Partition.GetNotificationHandler();
            SendWithSender(f.Partition.GetNotificationHandler(), f);
        } else
            SendWithSender(_handlerMemoization.GetValueOrDefault(correspondingHandler, correspondingHandler), result);
    }
    
    private T Clone<T>(T node) where T : class?, IReadableNode? =>
        (T)(_memoization.TryGetValue(node, out var result)
            ? result
            : _memoization[node] = SameIdCloner.Clone((INode)node));
}
```

Forest replicator replicates notifications for a local forest and all its partitions.

```csharp
var moved = new Documentation("moved");
var originPartition = new Geometry("origin-geo") { Shapes = [new Line("l") { ShapeDocs = moved }] };

var partition = new Geometry("geo");

var originalForest = new Forest();
var cloneForest = new Forest();

var sender = originalForest.GetNotificationHandler();
var cloneHandler = new TestNodeCloneNotificationHandler("forestCloner");
INotificationHandler.Connect(from: sender, to: cloneHandler);

var replicator = ForestReplicator.Create(cloneForest, new SharedNodeMap(), sender: null);
INotificationHandler.Connect(from: cloneHandler, to: replicator);

var receiver = new TestForestChangeNotificationHandler(originalForest, cloneHandler);
INotificationHandler.Connect(from: sender, to: receiver);

// Changes trigger PartitionAddedNotification and ChildMovedFromOtherContainmentNotification notifications 
originalForest.AddPartitions([partition, originPartition]);
partition.Documentation = moved;
```

```csharp
internal class TestForestChangeNotificationHandler(object? sender, TestNodeCloneNotificationHandler cloneHandler)
    : NotificationHandlerBase(sender), IReceivingNotificationHandler
{
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        switch (notification)
        {
            case PartitionAddedNotification partitionAddedNotification:
                OnLocalPartitionAdded(partitionAddedNotification);
                break;
        }
    }

    private void OnLocalPartitionAdded(PartitionAddedNotification partitionAddedNotification) => 
        INotificationHandler.Connect(from: partitionAddedNotification.NewPartition.GetNotificationHandler(), to: cloneHandler);
}
```