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

namespace LionWeb.Core.Benchmark;

using BenchmarkDotNet.Attributes;
using M1;
using Test.Serialization;
using Test.Utilities;

[MemoryDiagnoser]
public class NodeIdBenchmark
{
    private const int N = 10000;
    private readonly string idString;

    public NodeIdBenchmark()
    {
        idString = StringRandomizer.RandomLength();
    }

    [Benchmark]
    public string RawString() =>
        new(idString);

    [Benchmark]
    public NodeIdX NodeId() =>
        new NodeIdX(idString);

    [Benchmark]
    public ICompressedId CompressedId_WithOriginal() =>
        ICompressedId.Create(idString, new CompressedIdConfig(Compress: true, KeepOriginal: true));

    [Benchmark]
    public ICompressedId CompressedId_WithoutOriginal() =>
        ICompressedId.Create(idString, new CompressedIdConfig(Compress: true, KeepOriginal: false));

    [Benchmark]
    public ICompressedId UncompressedId() =>
        ICompressedId.Create(idString, new CompressedIdConfig(Compress: false, KeepOriginal: true));
}