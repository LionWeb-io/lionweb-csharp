// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace Io.Lionweb.Mps.Specific;

using LionWeb.Core;
using LionWeb.Core.M3;

#pragma warning disable 1591
public interface ISpecificLanguage : Language
{
    static ISpecificLanguage Get(LionWebVersions lionWebVersion) => lionWebVersion switch
    {
        IVersion2023_1 => LionWeb.Generator.VersionSpecific.V2023_1.SpecificLanguage.Instance,
        IVersion2024_1 => LionWeb.Generator.VersionSpecific.V2024_1.SpecificLanguage.Instance,
        IVersion2024_1_Compatible => LionWeb.Generator.VersionSpecific.V2024_1.SpecificLanguage.Instance,
        _ => throw new UnsupportedVersionException(lionWebVersion)
    };
    
    /// Key of all LionWeb BuiltIns language implementations.
    public const string LanguageKey = "LionCore-builtins";

    /// Name of all LionWeb BuiltIns language implementations.
    protected const string LanguageName = "LionCore_builtins";
    
    Annotation ConceptDescription { get; }
    Property ConceptDescription_conceptAlias { get; }
    Property ConceptDescription_conceptShortDescription { get; }
    Annotation Deprecated { get; }
    Property Deprecated_build { get; }
    Property Deprecated_comment { get; }
    Annotation ShortDescription { get; }
    Property ShortDescription_description { get; }
    Annotation VirtualPackage { get; }
}