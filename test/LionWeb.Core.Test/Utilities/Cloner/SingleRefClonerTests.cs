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

namespace LionWeb.Core.Test.Utilities.Cloner;

using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SingleRefClonerTests
{

    [TestMethod]
    public void IncludedBefore()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        var offsetDuplicate = ShapesLanguage.Instance.GetFactory().CreateOffsetDuplicate();
        offsetDuplicate.Source = line;

        var actual = Cloner.Clone([line, offsetDuplicate]).ToList();

        Assert.IsInstanceOfType<Line>(actual.First());
        var actualLine = actual.First() as Line;
        Assert.AreNotSame(line, actualLine);
        Assert.AreNotEqual(line.GetId(), actualLine.GetId());
        Assert.AreEqual("MyLine", actualLine.Name);

        Assert.IsInstanceOfType<OffsetDuplicate>(actual.Last());
        var actualDuplicate = actual.Last() as OffsetDuplicate;
        Assert.AreNotSame(offsetDuplicate, actualDuplicate);
        Assert.AreNotEqual(offsetDuplicate.GetId(), actualDuplicate.GetId());

        Assert.AreSame(actualLine, actualDuplicate.Source);
        Assert.AreSame(actualLine.GetId(), actualDuplicate.Source.GetId());
    }

    [TestMethod]
    public void IncludedAfter()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        var offsetDuplicate = ShapesLanguage.Instance.GetFactory().CreateOffsetDuplicate();
        offsetDuplicate.Source = line;

        var actual = Cloner.Clone([offsetDuplicate, line]).ToList();

        Assert.IsInstanceOfType<Line>(actual.Last());
        var actualLine = actual.Last() as Line;
        Assert.AreNotSame(line, actualLine);
        Assert.AreNotEqual(line.GetId(), actualLine.GetId());
        Assert.AreEqual("MyLine", actualLine.Name);

        Assert.IsInstanceOfType<OffsetDuplicate>(actual.First());
        var actualDuplicate = actual.First() as OffsetDuplicate;
        Assert.AreNotSame(offsetDuplicate, actualDuplicate);
        Assert.AreNotEqual(offsetDuplicate.GetId(), actualDuplicate.GetId());

        Assert.AreSame(actualLine, actualDuplicate.Source);
        Assert.AreSame(actualLine.GetId(), actualDuplicate.Source.GetId());
    }

    [TestMethod]
    public void Included_DoubleTarget()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        var offsetDuplicateA = ShapesLanguage.Instance.GetFactory().CreateOffsetDuplicate();
        offsetDuplicateA.Source = line;

        var offsetDuplicateB = ShapesLanguage.Instance.GetFactory().CreateOffsetDuplicate();
        offsetDuplicateB.Source = line;

        var actual = Cloner.Clone([offsetDuplicateA, line, offsetDuplicateB]).ToList();

        Assert.IsInstanceOfType<Line>(actual[1]);
        var actualLine = actual[1] as Line;
        Assert.AreNotSame(line, actualLine);
        Assert.AreNotEqual(line.GetId(), actualLine.GetId());
        Assert.AreEqual("MyLine", actualLine.Name);

        Assert.IsInstanceOfType<OffsetDuplicate>(actual[2]);
        var actualDuplicateA = actual[2] as OffsetDuplicate;
        Assert.AreNotSame(offsetDuplicateA, actualDuplicateA);
        Assert.AreNotEqual(offsetDuplicateA.GetId(), actualDuplicateA.GetId());
        Assert.AreSame(actualLine, actualDuplicateA.Source);
        Assert.AreSame(actualLine.GetId(), actualDuplicateA.Source.GetId());

        Assert.IsInstanceOfType<OffsetDuplicate>(actual[2]);
        var actualDuplicateB = actual[2] as OffsetDuplicate;
        Assert.AreNotSame(offsetDuplicateB, actualDuplicateB);
        Assert.AreNotEqual(offsetDuplicateB.GetId(), actualDuplicateB.GetId());
        Assert.AreSame(actualLine, actualDuplicateB.Source);
        Assert.AreSame(actualLine.GetId(), actualDuplicateB.Source.GetId());
    }

    [TestMethod]
    public void NoExternal()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        var offsetDuplicate = ShapesLanguage.Instance.GetFactory().CreateOffsetDuplicate();
        offsetDuplicate.Source = line;

        var cloner = new Cloner([offsetDuplicate]);
        cloner.KeepExternalReferences = false;
        var actual = cloner.Clone().Values.ToList();

        Assert.IsInstanceOfType<OffsetDuplicate>(actual.First());
        var actualDuplicate = actual.First() as OffsetDuplicate;
        Assert.AreNotSame(offsetDuplicate, actualDuplicate);
        Assert.AreNotEqual(offsetDuplicate.GetId(), actualDuplicate.GetId());

        Assert.ThrowsException<UnsetFeatureException>(() => actualDuplicate.Source);
    }

    [TestMethod]
    public void External()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        var offsetDuplicate = ShapesLanguage.Instance.GetFactory().CreateOffsetDuplicate();
        offsetDuplicate.Source = line;

        var cloner = new Cloner([offsetDuplicate]);
        cloner.KeepExternalReferences = true;
        var actual = cloner.Clone().Values.ToList();
        Assert.AreEqual(1, actual.Count);

        Assert.IsInstanceOfType<OffsetDuplicate>(actual.First());
        var actualDuplicate = actual.First() as OffsetDuplicate;
        Assert.AreNotSame(offsetDuplicate, actualDuplicate);
        Assert.AreNotEqual(offsetDuplicate.GetId(), actualDuplicate.GetId());

        Assert.AreSame(line, actualDuplicate.Source);
        Assert.AreSame(line.GetId(), actualDuplicate.Source.GetId());
    }
}