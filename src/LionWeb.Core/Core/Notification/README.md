# LionWeb notification system API

This document explains the LionWeb notification system API through some use cases. 

## Use cases
### How to get informed about changes
Every partition node (aka root node) of a model, which supports notification API, triggers a notification when there is a change to the model. 
In the example below, a partition is connected to a receiver. Receiver will be informed about all the changes to the partition via notifications. 
In this case, receiver (see `NotificationCounter` class definition below) counts the received notifications in its `Receive` method. 

Code below gives an example of API usage demonstrating how to get informed about changes to a partition.
```csharp
var partition = new Geometry("geo");
var receiver = new NotificationCounter();

// If notifications are not supported, partition.GetNotificationSender() returns null.
partition.GetNotificationSender()?.ConnectTo(receiver);
        
// This is a change to the model
partition.Documentation = new Documentation("added");
```
Code below gives an example of API usage demonstrating how to get informed about changes to a forest.
A forest is a collection of model trees, represented by each tree's partition.
```csharp
var forest = new Forest();
var receiver = new NotificationCounter();

// If notifications are not supported, forest.GetNotificationSender() returns null.
forest.GetNotificationSender()?.ConnectTo(receiver);

var partition = new Geometry("geo");

// This is a change to the forest
forest.AddPartitions([partition]); 
```

```csharp
class NotificationCounter() : NotificationPipeBase(null), INotificationHandler
{
    public int Count { get; private set; }

    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        Count++;
        Send(notification);
    }
}
```

### How to collect multiple changes into one change set 
Notifications raised by multiple changes to a model can be collected into one change set. 
A composite notification composes other forest and/or partition notifications into one
composite notification. Follow the comments below in the code blocks for further explanation.


```csharp
var partition = new Geometry("geo");

// NotificationCompositor implements composite notification logic. 
var compositor = new NotificationCompositor("compositor");

// Connects partition notification sender to compositor.
// If notifications are not supported, partition.GetNotificationSender() returns null.
partition.GetNotificationSender()?.ConnectTo(compositor);

// Push creates a new composite notification to collect incoming notifications
compositor.Push();
// Updates take place
UpdateDocumentation(partition);
// Pop returns the composite notification 
var changes = compositor.Pop(true);
    
Console.WriteLine($"Number of collected notifications : {changes.Parts.Count}"); // prints 2

// Access the notifications (changes)  
foreach (INotification notification in changes.Parts)
{
    Console.WriteLine(notification.ToString());
}
```

`UpdateDocumentation` is the method that applies changes to the partition.
```csharp
public void UpdateDocumentation(Geometry partition)
{
    // changes to the partition
    partition.Documentation = new Documentation("documentation");
    partition.Documentation.Text = "hello";
}
```

### How to replicate changes

Partition replicator replicates received notifications on a local equivalent partitions.

```csharp
var circle = new Circle("c");
var partition = new Geometry("geo") { Shapes = [circle] };
var clone = Clone(partition);

// Replicates notifications for the cloned partition. In this example, 
// PropertyAddedNotification is received for the partition and 
// replicator adds the same property value to the cloned partition node.
var replicator = PartitionReplicator.Create(clone, new SharedNodeMap(), partition.GetId());

// If notifications are not supported, partition.GetNotificationSender() returns null.
partition.GetNotificationSender()?.ConnectTo(replicator);

// This change triggers PropertyAddedNotification notification
circle.Name = "Hello";

```

Forest replicator replicates notifications for a local forest and all its partitions.

```csharp
var originalForest = new Forest();
var cloneForest = new Forest();

// Replicates notifications for the cloned forest. In this example, 
// PropertyAddedNotification and ChildMovedFromOtherContainmentNotification are received.
// Replicator adds the same partitions to the cloned forest and property value to the partition in the cloned forest.
var replicator = ForestReplicator.Create(cloneForest, new SharedNodeMap(), null);

// If notifications are not supported, forest.GetNotificationSender() returns null.
originalForest.GetNotificationSender()?.ConnectTo(replicator);

var moved = new Documentation("moved");
var originPartition = new Geometry("origin-geo") { Shapes = [new Line("l") { ShapeDocs = moved }] };
var partition = new Geometry("geo");

// Changes trigger PartitionAddedNotification and ChildMovedFromOtherContainmentNotification notifications
originalForest.AddPartitions([partition, originPartition]);
partition.Documentation = moved;
```
 