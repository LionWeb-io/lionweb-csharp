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

namespace LionWeb.Protocol.Delta;

using Core.M1;
using Core.M2;
using Core.M3;
using Core.Serialization;

public class DeltaUtils
{
    public static Dictionary<CompressedMetaPointer, IKeyed> BuildSharedKeyMap(IEnumerable<Language> languages)
    {
        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap = [];

        foreach (IKeyed keyed in languages.SelectMany(l => M1Extensions.Descendants<IKeyed>(l)))
        {
            var metaPointer = keyed switch
            {
                LanguageEntity l => l.ToMetaPointer(),
                Feature feat => feat.ToMetaPointer(),
                EnumerationLiteral l => l.GetEnumeration().ToMetaPointer(),
                Field f => f.ToMetaPointer(),
                _ => throw new NotImplementedException(keyed.GetType().Name)
            };

            sharedKeyedMap[CompressedMetaPointer.Create(metaPointer, true)] = keyed;
        }

        return sharedKeyedMap;
    }
}