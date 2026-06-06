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

namespace LionWeb.Core.M1;

/// Checks for duplicate node ids in an efficient manner.
public class DuplicateIdChecker
{
    private readonly HashSet<NodeId> _knownIds = new();

    /// Whether <paramref name="compressedId"/> has been seen before by this instance.
    [Obsolete("Use IsIdDuplicate(NodeId) instead.")]
    public bool IsIdDuplicate(ICompressedId compressedId) => 
        !_knownIds.Add(compressedId.AssertedOriginal);

    /// Whether <paramref name="nodeId"/> has been seen before by this instance.
    public bool IsIdDuplicate(NodeId nodeId) =>
        !_knownIds.Add(nodeId);
}