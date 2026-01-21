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

namespace LionWeb.Core.Test.NodeApi;

using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class WithAnnotationTests
{
    [TestMethod]
    public void AddAnnotation()
    {
        var node = new LinkTestConcept("node");
        Assert.IsEmpty(node.GetAnnotations());

        var ann = new TestAnnotation("ann");
        var withAnnotation = node.WithAnnotation(ann);
        
        Assert.AreSame(node, withAnnotation);
        Assert.Contains(ann, node.GetAnnotations());
        Assert.AreSame(node, ann.GetParent());
    }
}