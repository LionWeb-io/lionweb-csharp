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

namespace LionWeb.Core.Test.Serialization.Protobuf.Streaming;

using Core.Serialization;
using M3;
using MetaPointerIndex = ulong;

public enum PredefinedMetaPointer : MetaPointerIndex
{
    StringType = 1,
    IntegerType = 2,
    BooleanType = 3,
    Node = 4,
    INamed = 5,
    INamedName = 6
}

public static class PredefinedMetaPointerExtensions
{
    public static readonly MetaPointerIndex Max = Enum.GetValues(typeof(PredefinedMetaPointer)).Cast<MetaPointerIndex>()
        .Max();

    public static bool TryGet(MetaPointer value, LionWebVersions version, out MetaPointerIndex result)
    {
        if (value.Language != version.BuiltIns.Key || value.Version != version.BuiltIns.Version)
        {
            result = 0;
            return false;
        }

        switch (value.Key)
        {
            case { } k when k == version.BuiltIns.String.Key:
                result = (MetaPointerIndex)PredefinedMetaPointer.StringType;
                return true;
            case { } k when k == version.BuiltIns.Integer.Key:
                result = (MetaPointerIndex)PredefinedMetaPointer.IntegerType;
                return true;
            case { } k when k == version.BuiltIns.Boolean.Key:
                result = (MetaPointerIndex)PredefinedMetaPointer.BooleanType;
                return true;
            case { } k when k == version.BuiltIns.Node.Key:
                result = (MetaPointerIndex)PredefinedMetaPointer.Node;
                return true;
            case { } k when k == version.BuiltIns.INamed.Key:
                result = (MetaPointerIndex)PredefinedMetaPointer.INamed;
                return true;
            case { } k when k == version.BuiltIns.INamed_name.Key:
                result = (MetaPointerIndex)PredefinedMetaPointer.INamedName;
                return true;
            default:
                result = 0;
                return false;
        }
    }

    public static bool TryGet(this PredefinedMetaPointer predefinedMetaPointer, LionWebVersions version,
        out IKeyed? result)
    {
        switch (predefinedMetaPointer)
        {
            case PredefinedMetaPointer.StringType:
                result = version.BuiltIns.String;
                return true;
            case PredefinedMetaPointer.IntegerType:
                result = version.BuiltIns.Integer;
                return true;
            case PredefinedMetaPointer.BooleanType:
                result = version.BuiltIns.Boolean;
                return true;
            case PredefinedMetaPointer.Node:
                result = version.BuiltIns.Node;
                return true;
            case PredefinedMetaPointer.INamed:
                result = version.BuiltIns.INamed;
                return true;
            case PredefinedMetaPointer.INamedName:
                result = version.BuiltIns.INamed_name;
                return true;
            default:
                result = null;
                return false;
        }
    }
}

class MetaPointerIndexer(LionWebVersions lionWebVersion, Action<MetaPointer, MetaPointerIndex> adder)
    : IndexCounterBase<MetaPointer>(PredefinedMetaPointerExtensions.Max + 1, adder)
{
    protected override bool TryGet(MetaPointer? candidate, out MetaPointerIndex result) =>
        PredefinedMetaPointerExtensions.TryGet(candidate, lionWebVersion, out result);
}

class MetaPointerLookup(LionWebVersions lionWebVersion) : IndexLookupBase<IKeyed>
{
    protected override bool TryGet(ulong idx, out IKeyed? candidate) => 
        ((PredefinedMetaPointer)idx).TryGet(lionWebVersion, out candidate);
}