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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.Test.Utilities.Comparer;

using Core.Utilities;
using M3;

[TestClass]
public class ComparerDynamicTests : ComparerTestsBase
{
    [TestMethod]
    public void Property_Both_Same()
    {
        var lionWebVersion = LionWebVersions.Current;
        var lang = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var concept = lang.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", concept);
        left.Set(lionWebVersion.BuiltIns.INamed_name, "myName");

        var right = new DynamicNode("right", concept);
        right.Set(lionWebVersion.BuiltIns.INamed_name, "myName");

        AreEqual(right, right);
    }

    [TestMethod]
    public void Property_Both_Different()
    {
        var lionWebVersion = LionWebVersions.Current;
        var lang = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var concept = lang.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", concept);
        left.Set(lionWebVersion.BuiltIns.INamed_name, "myName");

        var right = new DynamicNode("right", concept);
        right.Set(lionWebVersion.BuiltIns.INamed_name, "yourName");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new PropertyValueDifference(left, "myName", _builtIns.INamed_name, right, "yourName") { Parent = parent }
        );
    }

    [TestMethod]
    public void Property_LeftOnly()
    {
        var lionWebVersion = LionWebVersions.Current;
        var lang = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var concept = lang.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", concept);
        left.Set(lionWebVersion.BuiltIns.INamed_name, "myName");

        var right = new DynamicNode("right", concept);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, _builtIns.INamed_name, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Property_RightOnly()
    {
        var lionWebVersion = LionWebVersions.Current;
        var lang = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var concept = lang.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", concept);

        var right = new DynamicNode("right", concept);
        right.Set(lionWebVersion.BuiltIns.INamed_name, "myName");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureLeftDifference(left, _builtIns.INamed_name, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Property_LeftOnly_DifferentLanguages()
    {
        var lionWebVersion = LionWebVersions.Current;
        var langA = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var conceptA = langA.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);
        var langB = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var conceptB = langB.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", conceptA);
        left.Set(lionWebVersion.BuiltIns.INamed_name, "myName");

        var right = new DynamicNode("right", conceptB);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, _builtIns.INamed_name, right) { Parent = parent }
        );
    }


    [TestMethod]
    public void Property_RightOnly_DifferentLanguages()
    {
        var lionWebVersion = LionWebVersions.Current;
        var langA = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var conceptA = langA.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);
        var langB = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var conceptB = langB.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", conceptA);

        var right = new DynamicNode("right", conceptB);
        right.Set(lionWebVersion.BuiltIns.INamed_name, "myName");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureLeftDifference(left, _builtIns.INamed_name, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Property_Both_Same_EmptyString()
    {
        var lionWebVersion = LionWebVersions.Current;
        var lang = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var concept = lang.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", concept);
        left.Set(lionWebVersion.BuiltIns.INamed_name, "");

        var right = new DynamicNode("right", concept);
        right.Set(lionWebVersion.BuiltIns.INamed_name, "");

        AreEqual(right, right);
    }

    [TestMethod]
    public void Property_LeftOnly_EmptyString()
    {
        var lionWebVersion = LionWebVersions.Current;
        var lang = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var concept = lang.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", concept);
        left.Set(lionWebVersion.BuiltIns.INamed_name, "");

        var right = new DynamicNode("right", concept);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, _builtIns.INamed_name, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Property_RightOnly_EmptyString()
    {
        var lionWebVersion = LionWebVersions.Current;
        var lang = new DynamicLanguage("LenientLang-id", lionWebVersion) { Name = "LenientLang", Key = "LenientLang-key", Version = "1" };
        var concept = lang.Concept("LenientConcept-id", "LenientConcept-key", "LenientConcept").Implementing(lionWebVersion.BuiltIns.INamed);

        var left = new DynamicNode("left", concept);

        var right = new DynamicNode("right", concept);
        right.Set(lionWebVersion.BuiltIns.INamed_name, "");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureLeftDifference(left, _builtIns.INamed_name, right) { Parent = parent }
        );
    }
}