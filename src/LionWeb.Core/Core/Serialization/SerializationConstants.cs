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

namespace LionWeb.Core.Serialization;

/// Singular place to define string values commonly used during serialization.
/// Should reduce memory usage and accelerate comparison (as identity check succeeds).
public static class SerializationConstants
{
    /// Empty string
    public static readonly string Empty = string.Empty;

    /// <c>0</c> as string
    public static readonly string Zero = string.Intern("0");

    /// <c>1</c> as string
    public static readonly string One = string.Intern("1");

    /// <c>true</c> as string
    public static readonly string True = string.Intern("true");

    /// <c>false</c> as string
    public static readonly string False = string.Intern("false");
}