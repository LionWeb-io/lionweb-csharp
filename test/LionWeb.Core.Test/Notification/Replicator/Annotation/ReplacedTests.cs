// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Notification.Replicator.Annotation;

using Core.Notification;
using Core.Notification.Partition;
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class ReplacedTests : ReplicatorTestsBase
{
    #region tests use ReplaceWith method

    [TestMethod]
    public void Multiple_Only_uses_ReplaceWith()
    {
        var replacement = new TestAnnotation("replacement");
        var replaced = new TestAnnotation("replaced");
        var originalPartition = new TestPartition("a");
        originalPartition.AddAnnotations([replaced]);

        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        Assert.IsTrue(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsFalse(sharedNodeMap.ContainsKey(replacement.GetId()));

        replaced.ReplaceWith(replacement);

        AssertEquals([originalPartition], [clonedPartition]);

        Assert.IsFalse(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(replacement.GetId()));
    }

    [TestMethod]
    public void Multiple_First_uses_ReplaceWith()
    {
        var replacement = new TestAnnotation("replacement");
        var replaced = new TestAnnotation("replaced");
        var originalPartition = new TestPartition("a");
        originalPartition.AddAnnotations([replaced, new TestAnnotation("bof")]);

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(replacement);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Last_uses_ReplaceWith()
    {
        var replacement = new TestAnnotation("replacement");
        var replaced = new TestAnnotation("replaced");
        var originalPartition = new TestPartition("a");
        originalPartition.AddAnnotations([new TestAnnotation("bof"), replaced]);

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(replacement);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion

    #region tests manually produce notifications

    [TestMethod]
    public void Multiple_Only()
    {
        var replaced = new TestAnnotation("replaced");
        var originalPartition = new TestPartition("a");
        originalPartition.AddAnnotations([replaced]);

        var clonedPartition = ClonePartition(originalPartition);

        var newAnnotation = new TestAnnotation("new");
        var annotationReplacedNotification = new AnnotationReplacedNotification(newAnnotation, replaced, originalPartition,
            0, new NumericNotificationId("annotationReplaced", 0));

        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(1, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(newAnnotation.GetId(), clonedPartition.GetAnnotations()[0].GetId());
    }

    [TestMethod]
    public void Multiple_First()
    {
        var replaced = new TestAnnotation("replaced");
        var originalPartition = new TestPartition("a");
        originalPartition.AddAnnotations([replaced, new TestAnnotation("bof")]);

        var clonedPartition = ClonePartition(originalPartition);

        var newAnnotation = new TestAnnotation("new");
        var annotationReplacedNotification = new AnnotationReplacedNotification(newAnnotation, replaced, originalPartition,
            0, new NumericNotificationId("annotationReplaced", 0));

        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(2, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(newAnnotation.GetId(), clonedPartition.GetAnnotations()[0].GetId());
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var replaced = new TestAnnotation("replaced");
        var originalPartition = new TestPartition("a");
        originalPartition.AddAnnotations([new TestAnnotation("bof"), replaced]);

        var clonedPartition = ClonePartition(originalPartition);

        var index = 1;
        var newAnnotation = new TestAnnotation("new");
        var annotationReplacedNotification = new AnnotationReplacedNotification(newAnnotation, replaced, originalPartition,
            index, new NumericNotificationId("annotationReplaced", 0));

        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(2, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(newAnnotation.GetId(), clonedPartition.GetAnnotations()[index].GetId());
    }
    
    [TestMethod]
    public void Multiple_not_matching_node_ids()
    {
        var replaced = new TestAnnotation("replaced");
        var nodeWithAnotherId = new TestAnnotation("node-with-another-id");
        var originalPartition = new TestPartition("a");
        originalPartition.AddAnnotations([new TestAnnotation("bof"), replaced, nodeWithAnotherId]);

        var clonedPartition = ClonePartition(originalPartition);

        var index = 1;
        var newAnnotation = new TestAnnotation("new");
        var notification = new AnnotationReplacedNotification(newAnnotation, nodeWithAnotherId, originalPartition,
            index, new NumericNotificationId("annotationReplaced", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }

    #endregion
}