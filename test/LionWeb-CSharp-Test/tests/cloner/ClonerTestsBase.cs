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

namespace LionWeb.Utils.Tests.Cloner;

using Core.M3;
using Examples.Shapes.M2;

public abstract class ClonerTestsBase
{
    protected class BadReferenceGeometry : ReferenceGeometry
    {
        private readonly IDictionary<Feature, object> _settings = new Dictionary<Feature, object>();

        public BadReferenceGeometry(string id) : base(id)
        {
        }

        public override IEnumerable<Feature> CollectAllSetFeatures() => _settings.Keys;

        protected override bool GetInternal(Feature feature, out object? result) =>
            _settings.TryGetValue(feature, out result);

        protected override bool SetInternal(Feature feature, object? value)
        {
            _settings[feature] = value;
            return true;
        }
    };

    protected class BadGeometry : Geometry
    {
        private readonly IDictionary<Feature, object> _settings = new Dictionary<Feature, object>();

        public BadGeometry(string id) : base(id)
        {
        }

        public override IEnumerable<Feature> CollectAllSetFeatures() => _settings.Keys;

        protected override bool GetInternal(Feature feature, out object? result) =>
            _settings.TryGetValue(feature, out result);

        protected override bool SetInternal(Feature feature, object? value)
        {
            _settings[feature] = value;
            return true;
        }
    };

    protected static Geometry CreateExampleGeometry(out Line line, out OffsetDuplicate duplicate, out Coord coord,
        out Documentation doc)
    {
        Geometry geometry = ShapesLanguage.Instance.GetFactory().CreateGeometry();

        line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        duplicate = ShapesLanguage.Instance.GetFactory().CreateOffsetDuplicate();
        duplicate.Name = "Duplicate";
        duplicate.Source = line;
        coord = ShapesLanguage.Instance.GetFactory().CreateCoord();
        coord.SetX(-3);
        coord.SetY(0);
        coord.SetZ(3);
        duplicate.SetOffset(coord);

        doc = ShapesLanguage.Instance.GetFactory().CreateDocumentation();
        doc.Text = "Slightly moved around";
        duplicate.AddAnnotations([doc]);

        geometry.AddShapes([line, duplicate]);
        return geometry;
    }
    
    protected static void AssertExampleGeometry(Geometry geometry)
    {
        Assert.AreEqual(2, geometry.Shapes.Count);

        Assert.IsInstanceOfType<Line>(geometry.Shapes.First());
        Line line = geometry.Shapes.First() as Line;
        Assert.AreSame(geometry, line.GetParent());
        Assert.AreEqual("MyLine", line.Name);

        Assert.IsInstanceOfType<OffsetDuplicate>(geometry.Shapes.Last());
        OffsetDuplicate duplicate = geometry.Shapes.Last() as OffsetDuplicate;
        Assert.AreSame(geometry, duplicate.GetParent());
        Assert.AreEqual("Duplicate", duplicate.Name);
        Assert.AreSame(line, duplicate.Source);

        Assert.IsInstanceOfType<Coord>(duplicate.Offset);
        Coord coord = duplicate.Offset;
        Assert.AreSame(duplicate, coord.GetParent());
        Assert.AreEqual(-3, coord.X);
        Assert.AreEqual(0, coord.Y);
        Assert.AreEqual(3, coord.Z);

        Assert.AreEqual(1, duplicate.GetAnnotations().Count);

        Assert.IsInstanceOfType<Documentation>(duplicate.GetAnnotations().First());
        Documentation doc = duplicate.GetAnnotations().First() as Documentation;
        Assert.AreSame(duplicate, doc.GetParent());
        Assert.AreEqual("Slightly moved around", doc.Text);
    }
}