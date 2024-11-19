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

namespace LionWeb.Core;

using M2;
using M3;

public abstract class LionWebVersions
{
    public abstract string VersionString { get; }
    public abstract IBuiltInsLanguage BuiltIns { get; }
    public abstract ILionCoreLanguage LionCore { get; }

    public static LionWebVersions Current => v2023_1;

    private LionWebVersions() {}
    
    public static readonly Version2023_1 v2023_1 = Version2023_1.Instance;
    public sealed class Version2023_1 : LionWebVersions
    {
        internal static readonly Version2023_1 Instance = new(); 
        
        private Version2023_1() {}
        
        public override string VersionString { get; } = "2023.1";

        public override IBuiltInsLanguage BuiltIns =>
            new Lazy<IBuiltInsLanguage>(() => BuiltInsLanguage_2023_1.Instance).Value;

        public override ILionCoreLanguage LionCore =>
            new Lazy<ILionCoreLanguage>(() => LionCoreLanguage_2023_1.Instance).Value;
    }

    public static readonly Version2024_1 v2024_1 = Version2024_1.Instance;
    public sealed class Version2024_1 : LionWebVersions
    {
        internal static readonly Version2024_1 Instance = new(); 
        private Version2024_1() {}
        
        public override string VersionString { get; } = "2024.1";

        public override IBuiltInsLanguage BuiltIns =>
            new Lazy<IBuiltInsLanguage>(() => BuiltInsLanguage_2024_1.Instance).Value;

        public override ILionCoreLanguage LionCore =>
            new Lazy<ILionCoreLanguage>(() => LionCoreLanguage_2024_1.Instance).Value;
    }
}

public interface ILionWebVersionUser<out TVersion, out TBuiltIns, out TM3>
    where TVersion: LionWebVersions
    where TBuiltIns : IBuiltInsLanguage<TVersion, TBuiltIns, TM3>
    where TM3 : ILionCoreLanguage<TVersion, TBuiltIns, TM3>
{
    TVersion ULionWebVersion { get; }
    protected TBuiltIns UBuiltIns { get; }
    protected TM3 UM3 { get; }
}
