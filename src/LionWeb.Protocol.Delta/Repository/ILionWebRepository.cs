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

namespace LionWeb.Protocol.Delta.Repository;

public interface ILionWebRepository
{
    private const string _cyan = "\x1b[96m";
    private const string _bold = "\x1b[1m";
    private const string _unbold = "\x1b[22m";
    private const string _defaultColor = "\x1b[39m";
    
    public const string HeaderColor_Start = _cyan + _bold;
    public const string HeaderColor_End =  _unbold + _defaultColor;
}