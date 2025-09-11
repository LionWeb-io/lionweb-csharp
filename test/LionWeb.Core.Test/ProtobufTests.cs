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

using Google.Protobuf;
using Io.Lionweb.Protobuf.Streaming;

[TestClass]
public class ProtobufTests
{
    [TestMethod]
    public void Test()
    {
        Print("empty", new PsTest());
        Print("req = 0", new PsTest() { Req = 0 });
        Print("opt = 0", new PsTest() { Opt = 0 });
        Print("req = 0, opt = 0", new PsTest() { Req = 0, Opt = 0 });
        Print("req = 1", new PsTest() { Req = 1 });
        Print("opt = 1", new PsTest() { Opt = 1 });
        Print("req = 1, opt = 1", new PsTest() { Req = 1, Opt = 1 });
    }

    private PsTest Print(string name, PsTest psTest)
    {
        var buf = psTest.ToByteArray();
        var bufContents = string.Join("", buf.Where(b => b != 0).Select(b => b.ToString("x2")));
        Console.WriteLine($"{name}:\n  toString: {psTest} req: {psTest.Req} opt: {psTest.Opt}\n  buf[{buf.Length}]: 0x{bufContents}");

        return psTest;
    }
}