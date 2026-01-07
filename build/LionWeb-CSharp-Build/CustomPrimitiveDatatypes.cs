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

namespace LionWeb.Build.Languages
{
    public record struct CustomType(string a, int b);

    namespace SubNamespace
    {
        public readonly struct CustomType(float c, bool d);
    }

    public struct CustomEqualityType(int i)
    {
        public int I { get; } = i;

        public static bool operator ==(CustomEqualityType self, CustomEqualityType other) =>
            self.I == other.I;

        public static bool operator !=(CustomEqualityType self, CustomEqualityType other) =>
            !(self == other);
    }

    public enum CustomEnumType
    {
        A, B, C
    }

    public sealed class CustomReferenceType
    {
        private string Value { get; init; }
    }

    public ref struct CustomRefStruct(string x, int y);
}