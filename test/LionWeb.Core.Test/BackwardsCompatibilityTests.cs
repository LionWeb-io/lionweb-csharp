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

namespace LionWeb.Core.Test;

using Languages;
using M2;

[TestClass]
public class BackwardsCompatibilityTests
{
    [TestMethod]
    public void NoRawReflection()
    {
        var parent = new LinkTestConcept("parent");
        var child = new LinkTestConcept("child");
        var annotationA = new TestAnnotation("annotationA");

        parent.AddAnnotations([annotationA]);
        Assert.AreSame(annotationA, parent.GetAnnotations().First());

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, child);
        Assert.AreSame(child, parent.Containment_1);

        parent.Set(parent.GetConcept().GetLanguage().LionWebVersion.BuiltIns.INamed_name, "hello");
        Assert.AreEqual("hello", parent.Name);

        var annotationB = new TestAnnotation("annotationB");
        parent.Set(null, new List<INode> { annotationB });
        Assert.AreSame(annotationB, parent.GetAnnotations().First());
    }
}