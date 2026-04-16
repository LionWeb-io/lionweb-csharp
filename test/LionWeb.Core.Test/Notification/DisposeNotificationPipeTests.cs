// Copyright 2026 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Pipe;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using System.Runtime.CompilerServices;

[TestClass]
public class DisposeNotificationPipeTests : NotificationTestsBase
{
    [TestMethod]
    public void SenderOnly()
    {
        object sender = new object();
        var member = new TestNotificationSender(sender);
        member.Dispose();
    }

    [TestMethod]
    public void ProducerOnly()
    {
        object sender = new object();
        var member = new TestNotificationProducer(sender);
        member.Dispose();
    }

    [TestMethod]
    public void ReceiverOnly()
    {
        var member = new TestNotificationReceiver();
        ((IDisposable)member).Dispose();
    }

    [TestMethod]
    public void SenderReceiver_Sender()
    {
        var receiver = new TestNotificationReceiver();
        var senderRef = Create(() => new TestNotificationSender(null));

        ConnectToDispose(senderRef, receiver);
        ForceGc();

        AssertCollected(senderRef);
    }

    [TestMethod]
    public void SenderReceiver_Receiver()
    {
        var sender = new TestNotificationSender(null);
        var receiverRef = Create(() => new TestNotificationReceiver());

        ConnectToDisconnect(sender, receiverRef);
        ForceGc();

        AssertCollected(receiverRef);
    }

    [TestMethod]
    public void SenderMember_Sender()
    {
        var receiver = new TestNotificationMember(null);
        var senderRef = Create(() => new TestNotificationSender(null));

        ConnectToDispose(senderRef, receiver);
        ForceGc();

        AssertCollected(senderRef);
    }

    [TestMethod]
    public void SenderMember_Member()
    {
        var sender = new TestNotificationSender(null);
        var receiverRef = Create(() => new TestNotificationMember(null));

        ConnectToDisconnect(sender, receiverRef);
        ForceGc();

        AssertCollected(receiverRef);
    }

    [TestMethod]
    public void Forest_WithPartition()
    {
        var forestRef = Create(() => new Forest());
        var partition = new TestPartition("part");

        Do(forestRef, f => f.AddPartitions([partition]));
        ForceGc();

        AssertAlive(forestRef);
    }

    [TestMethod]
    public void Forest_Disposed()
    {
        var forestRef = Create(() => new Forest());
        var partition = new TestPartition("part");

        Do(forestRef, f => f.AddPartitions([partition]));
        Do(forestRef, f => f.Dispose());
        ForceGc();

        AssertCollected(forestRef);
    }

    [TestMethod]
    public void Partition()
    {
        var forest = new Forest();
        var partitionRef = Create(() => new TestPartition("part"));

        Do(partitionRef, p => forest.AddPartitions([p]));
        Do(partitionRef, p => forest.RemovePartitions([p]));
        ForceGc();

        AssertCollected(partitionRef);
    }

    [TestMethod]
    public void ForestReplicator_DisposeSource()
    {
        var forestARef = Create(() => new Forest());
        var forestReplicatorRef = Ret(forestARef, f => Create(() => ForestReplicator.Create(f, new SharedNodeMap(), null)));
        var forestB = new Forest();
        Do(forestReplicatorRef, r => forestB.GetNotificationSender()!.ConnectTo(r));
        forestB.Dispose();

        ForceGc();

        AssertCollected(forestARef);
        AssertCollected(forestReplicatorRef);
    }

    [TestMethod]
    public void ForestReplicator_Disposed()
    {
        var forestARef = Create(() => new Forest());
        var forestReplicatorRef = Ret(forestARef, f => Create(() => ForestReplicator.Create(f, new SharedNodeMap(), null)));
        var forestB = new Forest();
        Do(forestReplicatorRef, r => forestB.GetNotificationSender()!.ConnectTo(r));
        Do(forestReplicatorRef, r =>
        {
            forestB.GetNotificationSender()!.Disconnect(r);
            r.Dispose();
        });

        ForceGc();

        AssertCollected(forestARef);
        AssertCollected(forestReplicatorRef);
    }

    private static void AssertCollected<T>(WeakReference<T> weakRef) where T : class
        => Assert.IsFalse(weakRef.TryGetTarget(out _));

    private static void AssertAlive<T>(WeakReference<T> weakRef) where T : class
        => Assert.IsTrue(weakRef.TryGetTarget(out _));

    private static void ForceGc()
    {
        // Force multiple cycles to ensure all generations are cleared
        for (int i = 0; i < 2; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private R Ret<A, R>(WeakReference<A> aRef, Func<A, R> func) where A : class
    {
        if (aRef.TryGetTarget(out var a))
        {
            return func(a);
        }
        
        Assert.Fail("WeakRef empty");
        return default;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Do<A>(WeakReference<A> aRef, Action<A> action) where A : class
    {
        if (aRef.TryGetTarget(out var a))
        {
            action(a);
            return;
        }
        
        Assert.Fail("WeakRef empty");
    }

    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Do<A, B>(WeakReference<A> aRef, WeakReference<B> bRef, Action<A, B> action) where A : class where B : class
    {
        if (aRef.TryGetTarget(out var a) && bRef.TryGetTarget(out var b))
        {
            action(a, b);
            return;
        }
        
        Assert.Fail("WeakRef empty");
    }

    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ConnectToDispose<T>(WeakReference<T> senderRef, INotificationReceiver receiver) where T : class, INotificationSender
    {
        if (senderRef.TryGetTarget(out var x))
        {
            var sender = ((INotificationSender)x);
            sender.ConnectTo(receiver);
            sender.Dispose();
        } else
        {
            Assert.Fail("WeakRef empty");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ConnectToDispose<T>(INotificationSender sender, WeakReference<T> receiverRef) where T : class, INotificationReceiver
    {
        if (receiverRef.TryGetTarget(out var x))
        {
            var receiver = ((INotificationReceiver)x);
            sender.ConnectTo(receiver);
            receiver.Dispose();
        } else
        {
            Assert.Fail("WeakRef empty");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ConnectToDisconnect<T>(WeakReference<T> senderRef, INotificationReceiver receiver) where T : class, INotificationSender
    {
        if (senderRef.TryGetTarget(out var x))
        {
            var sender = ((INotificationSender)x);
            sender.ConnectTo(receiver);
            sender.Disconnect(receiver);
        } else
        {
            Assert.Fail("WeakRef empty");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ConnectToDisconnect<T>(INotificationSender sender, WeakReference<T> receiverRef) where T : class, INotificationReceiver
    {
        if (receiverRef.TryGetTarget(out var x))
        {
            var receiver = ((INotificationReceiver)x);
            sender.ConnectTo(receiver);
            sender.Disconnect(receiver);
        } else
        {
            Assert.Fail("WeakRef empty");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private WeakReference<T> Create<T>(Func<T> creator) where T : class
    {
        var obj = creator();
        return new WeakReference<T>(obj);
    }

    private class TestNotificationSender(object? sender) : NotificationPipeBase(sender);

    private class TestNotificationProducer(object? sender) : NotificationPipeBase(sender), INotificationProducer
    {
        public void ProduceNotification(INotification notification) => throw new NotImplementedException();
    }

    private class TestNotificationMember(object? sender) : NotificationPipeBase(sender), INotificationReceiver
    {
        public void Receive(INotificationSender correspondingSender, INotification notification) => Send(notification);
    }


    private class TestNotificationReceiver : INotificationReceiver
    {
        public void Receive(INotificationSender correspondingSender, INotification notification) =>
            throw new NotImplementedException();
    }
}