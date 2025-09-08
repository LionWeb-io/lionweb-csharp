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

using Core.Utilities;
using M3;

public enum PredefinedLanguage : LanguageIndex
{
    BuiltIns = 1,
    LionCore = 2
}

public static class PredefinedLanguageExtensions
{
    public static readonly LanguageIndex Max = Enum.GetValues(typeof(PredefinedLanguage)).Cast<LanguageIndex>().Max();

    public static bool TryGet(Language value, LionWebVersions version, out LanguageIndex result)
    {
        switch (value)
        {
            case { } v when v.EqualsIdentity(version.BuiltIns):
                result = (LanguageIndex)PredefinedLanguage.BuiltIns;
                return true;
            case { } v when v.EqualsIdentity(version.LionCore):
                result = (LanguageIndex)PredefinedLanguage.LionCore;
                return true;
            default:
                result = 0;
                return false;
        }
    }

    public static bool TryGet(this PredefinedLanguage predefinedLanguage, LionWebVersions version, out Language? result)
    {
        switch (predefinedLanguage)
        {
            case PredefinedLanguage.BuiltIns:
                result = version.BuiltIns;
                return true;
            case PredefinedLanguage.LionCore:
                result = version.LionCore;
                return true;
            default:
                result = null;
                return false;
        }
    }
}

class LanguageIndexer(LionWebVersions lionWebVersion, Action<Language, LanguageIndex> adder)
    : IndexCounterBase<Language>(PredefinedLanguageExtensions.Max + 1, adder, new LanguageIdentityComparer())
{
    protected override bool TryGet(Language? candidate, out LanguageIndex result) =>
        PredefinedLanguageExtensions.TryGet(candidate, lionWebVersion, out result);
}

class LanguageLookup(LionWebVersions lionWebVersion) : IndexLookupBase<Language>(PredefinedLanguageExtensions.Max + 1)
{
    protected override bool TryGet(LanguageIndex idx, out Language? candidate) => 
        ((PredefinedLanguage)idx).TryGet(lionWebVersion, out candidate);
}