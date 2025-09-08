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

// If notifications are not supported, partition.GetNotificationSender() returns null.
var sender = partition.GetNotificationSender();

// Connects partition notification sender to compositor.
sender?.ConnectTo(compositor);

// Push creates a new composite notification to collect incoming notifications
compositor.Push();
// Updates take place
UpdateDocumentation(partition);
// Pop returns the composite notification 
var changes = compositor.Pop();
    
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

#### Partition replicator

Partition replicator replicates received changes (via notifications) on a local equivalent partitions.  
Follow the comments below in the code blocks for further explanation.

```csharp
// Changes will be applied to this local partition
var localPartition = new Geometry("geo");

// Gets changes on this local partition
IEnumerable<INotification> changes = GetChangesOn(localPartition);

// Replicates received changes on local partition 
ReplicateChangesOn(localPartition, changes);
```

`GetChangesOn` emulates the source of notifications by manually creating two notifications. 
Any other source would also work. 
```csharp
public IEnumerable<INotification> GetChangesOn(Geometry localPartition)
{
    var documentation = new Documentation("documentation");
    
    return
    [
        new ChildAddedNotification(localPartition, documentation,
            ShapesLanguage.Instance.Geometry_documentation, 0, new NumericNotificationId("ChildAddedNotification", 0)),
        
        new PropertyAddedNotification(documentation, ShapesLanguage.Instance.Documentation_text, 
            "hello", new NumericNotificationId("PropertyAddedNotification", 0))
    ];
}
```

`ReplicateChangesOn` replicates the changes on local partition.
```csharp
public void ReplicateChangesOn(Geometry localPartition, IEnumerable<INotification> changes)
{
    // Creates a partition replicator
    var replicator = PartitionReplicator.Create(localPartition, new SharedNodeMap(), "partition replicator");

    // Creater acts as a remote notificaiton producer
    var creator = new Creator();
    
    // Replicator will receive changes form the creator 
    creator.ConnectTo(replicator);

    // Creater forwards the changes to the replicator
    foreach (var notification in changes)
    {
        creator.ProduceNotification(notification);
    }
}
```

```csharp
public class Creator() : NotificationPipeBase(null), INotificationProducer
{
    public void ProduceNotification(INotification notification) => Send(notification);
}
```

#### Forest replicator

Forest replicator replicates notifications for a local forest and all its partitions.

```csharp
// Changes will be applied to this local forest
var localForest = new Forest();

// Gets changes
IEnumerable<INotification> changes = GetChanges();

// Replicates changes on local forest
ReplicateChangesOn(localForest, changes);
```

`GetChanges` adds a new partition to the forest. Then it adds a new child to the partition and sets its property. 
```csharp
public IEnumerable<INotification> GetChanges()
{
    var partition = new Geometry("geo");
    var documentation = new Documentation("documentation");
    
    return
    [
        new PartitionAddedNotification(partition, new NumericNotificationId("PartitionAddedNotification", 0)),
        
        new ChildAddedNotification(partition, documentation,
            ShapesLanguage.Instance.Geometry_documentation, 0, new NumericNotificationId("ChildAddedNotification", 0)),
        
        new PropertyAddedNotification(documentation, ShapesLanguage.Instance.Documentation_text, 
            "hello", new NumericNotificationId("PropertyAddedNotification", 0))
    ];
}
```

`ReplicateChangesOn` replicates the changes on local forest.
```csharp
public void ReplicateChangesOn(Forest localForest, IEnumerable<INotification> changes)
{
    // Creates a forest replicator
    var replicator = ForestReplicator.Create(localForest, new SharedNodeMap(), "forest replicator");

    // Creater acts as a remote notificaiton producer
    var creator = new Creator();

    // Replicator will receive changes form the creator 
    creator.ConnectTo(replicator);

    // Creater forwards the changes to the replicator
    foreach (var notification in changes)
    {
        creator.ProduceNotification(notification);
    }
}
```