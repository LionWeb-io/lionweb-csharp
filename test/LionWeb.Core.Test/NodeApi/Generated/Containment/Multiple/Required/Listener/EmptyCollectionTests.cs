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

using LionWeb.Core.Notification.Partition;
using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using LionWeb.Core.Test.Notification;

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.Listener;

[TestClass]
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => compositeShape.AddParts(values));

        observer.AssertNone<ChildAddedNotification>();
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        var values = new IShape[0];
        Assert.ThrowsExactly<InvalidValueException>(
            () => compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values));

        observer.AssertNone<ChildAddedNotification>();
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => compositeShape.InsertParts(0, values));

        observer.AssertNone<ChildAddedNotification>();
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => compositeShape.RemoveParts(values));

        observer.AssertEmpty();
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var value = new Circle("myId");
        compositeShape.AddParts([value]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        var values = new List<Coord>();
        Assert.ThrowsExactly<InvalidValueException>(
            () => compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values));

        observer.AssertEmpty();
    }
}
