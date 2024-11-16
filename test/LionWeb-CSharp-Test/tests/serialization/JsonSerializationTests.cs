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

namespace LionWeb_CSharp_Test.tests.serialization;

using System.Text.Json;

[TestClass]
public class JsonSerializationTests
{
    [TestMethod]
    public void CamelCasedEquivalenceTest()
    {
        var json = "{\"foo\":\"bar\"}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var myStruct = JsonSerializer.Deserialize<Struct>(json, options);
        Assert.AreEqual(new Struct { Foo = "bar" }, myStruct);
        var serJson = JsonSerializer.Serialize(myStruct, options);
        Assert.AreEqual(json, serJson);
    }
}

internal class Struct
{
    public String Foo { get; set; }

    public override string ToString()
    {
        return $"Struct(foo=\"{Foo}\")";
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Struct)obj);
    }

    protected bool Equals(Struct other)
    {
        return Foo == other.Foo;
    }

    public override int GetHashCode()
    {
        return Foo.GetHashCode();
    }
}
