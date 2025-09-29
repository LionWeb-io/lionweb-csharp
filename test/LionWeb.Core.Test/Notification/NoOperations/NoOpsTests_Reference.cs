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

namespace LionWeb.Core.Test.Notification.NoOperationsTests;

using Languages.Generated.V2025_1.Shapes.M2;
using M1;

[TestClass]
public class Reference: NotificationTestsBase
{
    [TestMethod]
    public void ReferenceChanged_to_the_same_target()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var originalPartition = new Geometry("a") { Shapes = [od, circle, line] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        od.AltSource = circle;
        
        Assert.AreEqual(0, notificationObserver.Count);
    }
    
    [TestMethod]
    public void ReferenceChanged_to_the_same_target_uses_ReplaceWith()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var originalPartition = new Geometry("a") { Shapes = [od, circle, line] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        od.AltSource.ReplaceWith(circle);
        
        Assert.AreEqual(0, notificationObserver.Count);
    }
    
}