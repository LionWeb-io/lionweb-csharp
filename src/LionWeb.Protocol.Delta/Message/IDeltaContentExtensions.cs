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

namespace LionWeb.Protocol.Delta.Message;

using Command;
using Event;

/// <summary>
/// Extensions for <see cref="IDeltaContent"/>.
/// </summary>
public static class IDeltaContentExtensions
{
    /// <summary>
    /// Recursively collects <paramref name="deltaContent"/> and all nested delta content.
    /// </summary>
    /// <param name="deltaContent">delta content to start collecting on.</param>
    /// <param name="includeSelf">Whether <paramref name="deltaContent"/> should be included in the result; defaults to <see langword="true"/>.</param>
    /// <returns>All nested delta contents for <paramref name="deltaContent"/> in top-down, depth-first order.</returns>
    /// <seealso cref="IDeltaComposite"/>
    /// <seealso cref="CompositeCommand"/>
    /// <seealso cref="CompositeEvent"/>
    public static List<IDeltaContent> CollectNested(this IDeltaContent deltaContent, bool includeSelf = true)
    {
        List<IDeltaContent> result = [];
        CollectNested(deltaContent, result);

        if (!includeSelf)
            result.Remove(deltaContent);

        return result;
    }

    private static void CollectNested(IDeltaContent deltaContent, List<IDeltaContent> nested)
    {
        if (nested.Contains(deltaContent))
            return;
        
        nested.Add(deltaContent);
        
        if (deltaContent is IDeltaComposite composite)
            foreach (var part in composite.CompositeParts)
            {
                CollectNested(part, nested);
            }
    }
}