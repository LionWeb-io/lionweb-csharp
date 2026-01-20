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

namespace LionWeb.Generator.Test;

using Core;
using Core.M2;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using GeneratorExtensions;
using Impl;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

[TestClass]
public class NodeBuilderTests
{
    [TestMethod]
    public void ValidNodeId()
    {
        var builder = new NodeBuilder();
        Assert.AreSame(builder, builder.WithNodeId("ab"));
    }

    [TestMethod]
    public void InvalidNodeId()
    {
        var builder = new NodeBuilder();
        Assert.Throws<InvalidIdException>(() => builder.WithNodeId("a b"));
    }

    [TestMethod]
    public void UnsetNodeId()
    {
        var builder = new NodeBuilder()
            .WithNodeType("a");
        Assert.Throws<InvalidIdException>(() => builder.Build());
    }

    [TestMethod]
    public void ValidNodeType_String()
    {
        var builder = new NodeBuilder();
        Assert.AreSame(builder, builder.WithNodeType("ab"));
    }

    [TestMethod]
    public void ValidNodeType_Type()
    {
        var builder = new NodeBuilder();
        Assert.AreSame(builder, builder.WithNodeType(typeof(INamed)));
    }

    [TestMethod]
    public void InvalidNodeType()
    {
        var builder = new NodeBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithNodeType("a b"));
    }

    [TestMethod]
    public void UnsetNodeType()
    {
        var builder = new NodeBuilder()
            .WithNodeId("a");
        Assert.Throws<ArgumentException>(() => builder.Build());
    }

    [TestMethod]
    public void NoInitializer()
    {
        var creation = new NodeBuilder()
            .WithNodeId("a")
            .WithNodeType("b")
            .Build();

        AssertEquals("""new b("a")""", creation);
    }

    [TestMethod]
    public void ValidInitializer_string()
    {
        var creation = new NodeBuilder()
            .WithNodeId("a")
            .WithNodeType("b")
            .WithInitializer("c", "hello")
            .Build();

        AssertEquals("""new b("a") { c = "hello" }""", creation);
    }

    [TestMethod]
    public void ValidInitializer_int()
    {
        var creation = new NodeBuilder()
            .WithNodeId("a")
            .WithNodeType("b")
            .WithInitializer("c", 5)
            .Build();

        AssertEquals("""new b("a") { c = 5 }""", creation);
    }

    [TestMethod]
    public void ValidInitializer_bool()
    {
        var creation = new NodeBuilder()
            .WithNodeId("a")
            .WithNodeType("b")
            .WithInitializer("c", true)
            .Build();

        AssertEquals("""new b("a") { c = true }""", creation);
    }

    [TestMethod]
    public void ValidInitializer_override()
    {
        var creation = new NodeBuilder()
            .WithNodeId("a")
            .WithNodeType("b")
            .WithInitializer("c", "a")
            .WithInitializer("c", "b")
            .Build();

        AssertEquals("""new b("a") { c = "b" }""", creation);
    }

    [TestMethod]
    public void ValidInitializer_nested()
    {
        var nested = new NodeBuilder()
            .WithNodeId("nA")
            .WithNodeType("nB")
            .WithInitializer("nC", true)
            .Build();
        var creation = new NodeBuilder()
            .WithNodeId("a")
            .WithNodeType("b")
            .WithInitializer("c", nested)
            .Build();

        AssertEquals("""new b("a") { c = new nB("nA") { nC = true } }""", creation);
    }

    [TestMethod]
    public void InvalidInitializerName_With()
    {
        var builder = new NodeBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithInitializer("a b", 1));
    }

    [TestMethod]
    public void InvalidInitializerName_Init()
    {
        var builder = new NodeBuilder() { Initializers = { { "a b", 1.AsLiteral() } } };
        Assert.Throws<ArgumentException>(() => builder.Build());
    }

    [TestMethod]
    public void InitializeInstance()
    {
        var instance = new DataTypeTestConcept("id")
        {
            BooleanValue_0_1 = true,
            BooleanValue_1 = false,
            StringValue_0_1 = "str",
            IntegerValue_0_1 = 1,
            IntegerValue_1 = 2
        };
        var creation = new NodeBuilder(instance)
            .Build();

        AssertEquals("""new DataTypeTestConcept("id") { BooleanValue_0_1 = true, BooleanValue_1 = false, IntegerValue_0_1 = 1, IntegerValue_1 = 2, StringValue_0_1 = "str" }""", creation);
    }

    [TestMethod]
    public void Join_empty()
    {
        AssertEquals("""[]""", NodeBuilder.Join([]));
    }

    [TestMethod]
    public void Join_single()
    {
        var node0 = new NodeBuilder().WithNodeId("a0").WithNodeType("b0").Build();
        AssertEquals("""[new b0("a0")]""", NodeBuilder.Join(node0));
    }

    [TestMethod]
    public void Join_multiple()
    {
        var node0 = new NodeBuilder().WithNodeId("a0").WithNodeType("b0").Build();
        var node1 = new NodeBuilder().WithNodeId("a1").WithNodeType("b1").Build();
        AssertEquals("""[new b0("a0"), new b1("a1")]""", NodeBuilder.Join(node0, node1));
    }


    private static void AssertEquals(string expected, ExpressionSyntax creation)
    {
        var workspace = new AdhocWorkspace();
        Assert.AreEqual(expected, Formatter.Format(creation, workspace).GetText().ToString());
    }
}