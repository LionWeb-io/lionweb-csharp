// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class ContainmentTests_Single_Optional_Listener
{
    #region Single

    [TestMethod]
    public void Single()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(doc, args.NewChild);
        });

        parent.Documentation = doc;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_Setter()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(doc, args.NewChild);
        });

        parent.SetDocumentation(doc);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(doc, args.NewChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_FromOtherParent()
    {
        var parent = new Geometry("g");

        var doc = new Documentation("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Docs = doc };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_docs, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(doc, args.MovedChild);
        });

        parent.Documentation = doc;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_FromOtherParent_Reflective()
    {
        var parent = new Geometry("g");

        var doc = new Documentation("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Docs = doc };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_docs, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(doc, args.MovedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_FromOtherParent_Replace()
    {
        var replacedDoc = new Documentation("replacedDoc");
        var parent = new Geometry("g") { Documentation = replacedDoc };

        var doc = new Documentation("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Docs = doc };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildMovedAndReplacedFromOtherContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_docs, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(doc, args.MovedChild);
            Assert.AreEqual(replacedDoc, args.ReplacedChild);
        });

        parent.Documentation = doc;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_FromOtherParent_Replace_Reflective()
    {
        var replacedDoc = new Documentation("replacedDoc");
        var parent = new Geometry("g") { Documentation = replacedDoc };

        var doc = new Documentation("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Docs = doc };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildMovedAndReplacedFromOtherContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_docs, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(doc, args.MovedChild);
            Assert.AreEqual(replacedDoc, args.ReplacedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_FromSameParent()
    {
        var doc = new Documentation("myId");
        var offsetDuplicate = new OffsetDuplicate("g") { SecretDocs = doc };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_secretDocs, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_docs, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(doc, args.MovedChild);
        });

        offsetDuplicate.Docs = doc;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_FromSameParent_Reflective()
    {
        var doc = new Documentation("myId");
        var offsetDuplicate = new OffsetDuplicate("g") { SecretDocs = doc };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_secretDocs, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_docs, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(doc, args.MovedChild);
        });

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, doc);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_FromSameParent_Replace()
    {
        var replacedDoc = new Documentation("replacedDoc");
        var doc = new Documentation("myId");
        var offsetDuplicate = new OffsetDuplicate("g") { SecretDocs = doc, Docs = replacedDoc };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_secretDocs, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_docs, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(doc, args.MovedChild);
            Assert.AreEqual(replacedDoc, args.ReplacedChild);
        });

        offsetDuplicate.Docs = doc;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Single_FromSameParent_Replace_Reflective()
    {
        var replacedDoc = new Documentation("replacedDoc");
        var doc = new Documentation("myId");
        var offsetDuplicate = new OffsetDuplicate("g") { SecretDocs = doc, Docs = replacedDoc };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_secretDocs, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_docs, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(doc, args.MovedChild);
            Assert.AreEqual(replacedDoc, args.ReplacedChild);
        });

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, doc);

        Assert.AreEqual(1, notifications);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };
        var doc = new Documentation("myId");

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildReplacedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(doc, args.NewChild);
            Assert.AreEqual(oldDoc, args.ReplacedChild);
        });

        parent.Documentation = doc;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };
        var doc = new Documentation("myId");

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildReplacedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(doc, args.NewChild);
            Assert.AreEqual(oldDoc, args.ReplacedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Existing_Same()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        parent.Documentation = oldDoc;

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Existing_Same_Reflective()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, oldDoc);

        Assert.AreEqual(0, notifications);
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new Geometry("g");

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        parent.Documentation = null;

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new Geometry("g");

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationHandler().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, null);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Null_Existing()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(oldDoc, args.DeletedChild);
        });

        parent.Documentation = null;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Null_Existing_Reflective()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };

        int notifications = 0;
        parent.GetNotificationHandler().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(oldDoc, args.DeletedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, null);

        Assert.AreEqual(1, notifications);
    }

    #endregion
}