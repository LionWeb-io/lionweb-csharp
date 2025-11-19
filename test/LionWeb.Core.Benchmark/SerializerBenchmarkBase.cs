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

using M3;
using Serialization;
using System.Text.Json;
using Test.Languages.Generated.V2024_1.Shapes.M2;

public class SerializerBenchmarkBase
{
    protected readonly Language _language;
    protected readonly LionWebVersions _lionWebVersion;

    public SerializerBenchmarkBase() : this(ShapesLanguage.Instance, LionWebVersions.Current)
    {
    }

    public SerializerBenchmarkBase(Language language, LionWebVersions lionWebVersion)
    {
        _language = language;
        _lionWebVersion = lionWebVersion;
    }

    // private const long _maxSize = 1_500_000L;
    protected const long _maxSize = 100_500L;
    protected const string _streamFile = "/tmp/output_stream.json";
    protected const string _stringFile = "/tmp/output_string.json";

    protected static readonly JsonSerializerOptions _aotOptions;
    protected static readonly JsonSerializerOptions _simpleOptions;

    static SerializerBenchmarkBase()
    {
        _aotOptions = LionWebJsonSerializerContext.Default.Options;
        _simpleOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    static string AsFraction(long value) =>
        $"{value / 1_000_000D:0.000}" + "M";
}