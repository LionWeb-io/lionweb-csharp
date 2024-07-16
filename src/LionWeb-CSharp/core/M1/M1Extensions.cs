﻿// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

using M2;
using M3;
using System.Collections;

/// <summary>
/// Extension methods for LionCore M2 types. 
/// </summary>
public static class M1Extensions
{
    /// <summary>
    /// Inserts <paramref name="newPredecessor"/> before <paramref name="self"/>.
    /// </summary>
    /// <param name="self">Base node, must be a child in a multiple containment.</param>
    /// <param name="newPredecessor">Node that will be inserted in the same containment, and before <paramref name="self"/>.</param>
    /// <exception cref="TreeShapeException">If <paramref name="self"/> has no parent, or is not a child in a multiple containment.</exception>
    public static void InsertBefore(this INode self, INode newPredecessor)
    {
        INode? parent = self.GetParent();
        if (parent == null)
            throw new TreeShapeException(self, "Cannot insert before a node with no parent");

        Containment containment = parent.GetContainmentOf(self)!;
        if (!containment.Multiple)
            throw new TreeShapeException(self, "Cannot insert before a node in a single containment");

        var value = parent.Get(containment);
        if (value is not IEnumerable enumerable)
            // should not happen
            throw new TreeShapeException(self, "Cannot insert before a node in a single containment");

        var list = new List<INode>(containment.AsNodes<INode>(enumerable));
        var index = list.IndexOf(self);
        if (index < 0)
            // should not happen
            throw new TreeShapeException(self, "Cannot insert before a node with no parent");

        list.Insert(index, newPredecessor);
        parent.Set(containment, list);
    }

    /// <summary>
    /// Inserts <paramref name="newSuccessor"/> after <paramref name="self"/>.
    /// </summary>
    /// <param name="self">Base node, must be a child in a multiple containment.</param>
    /// <param name="newSuccessor">Node that will be inserted in the same containment, and after <paramref name="self"/>.</param>
    /// <exception cref="TreeShapeException">If <paramref name="self"/> has no parent, or is not a child in a multiple containment.</exception>
    public static void InsertAfter(this INode self, INode newSuccessor)
    {
        INode? parent = self.GetParent();
        if (parent == null)
            throw new TreeShapeException(self, "Cannot insert after a node with no parent");

        Containment containment = parent.GetContainmentOf(self)!;
        if (!containment.Multiple)
            throw new TreeShapeException(self, "Cannot insert after a node in a single containment");

        var value = parent.Get(containment);
        if (value is not IEnumerable enumerable)
            // should not happen
            throw new TreeShapeException(self, "Cannot insert after a node in a single containment");

        var list = new List<INode>(containment.AsNodes<INode>(enumerable));
        var index = list.IndexOf(self);
        if (index < 0)
            // should not happen
            throw new TreeShapeException(self, "Cannot insert after a node with no parent");

        list.Insert(index + 1, newSuccessor);
        parent.Set(containment, list);
    }

    /// <summary>
    /// Returns all the preceding siblings of the base node <param name="self"></param>
    /// Optionally includes <paramref name="self"/>.
    /// 
    /// Starting from index 0 to the index of base node, includes the index of <param name="self"></param>
    /// if <paramref name="includeSelf"/> is set to true.  
    /// </summary>
    /// <param name="self">Base node to find preceding siblings of.</param>
    /// <param name="includeSelf">If true, the result includes <paramref name="self"/>.</param>
    /// <returns>List of preceding siblings of <param name="self"></param>.</returns>
    /// <exception cref="TreeShapeException">If <paramref name="self"/> has no parent, or is not a child in a multiple containment.</exception>
    public static IEnumerable<INode> PrecedingSiblings(this INode self, bool includeSelf = false)
    {
        List<INode> list = GetContainmentNodes(self);
        var takeCount = includeSelf ? list.IndexOf(self) + 1 : list.IndexOf(self);
        return list.Take(takeCount);
    }

    /// <summary>
    /// Returns all the following siblings of the base node <param name="self"></param>
    /// Optionally includes <paramref name="self"/>.
    ///
    /// Starting from the following index of <param name="self"></param>, includes the index of <paramref name="self"/>
    /// if <paramref name="includeSelf"/> is set to true.  
    /// </summary>
    ///
    /// <param name="self">Base node to find following siblings of.</param>
    /// <param name="includeSelf">If true, the result includes <paramref name="self"/>.</param>
    /// <returns>List of following siblings of <param name="self"></param>.</returns>
    /// <exception cref="TreeShapeException">If <paramref name="self"/> has no parent, or is not a child in a multiple containment.</exception>
    public static IEnumerable<INode> FollowingSiblings(this INode self, bool includeSelf = false)
    {
        List<INode> list = GetContainmentNodes(self);
        var skipCount = includeSelf ? list.IndexOf(self) : list.IndexOf(self) + 1;
        return list.Skip(skipCount);
    }

    /// <summary>
    /// Replaces <paramref name="self"/> in its parent with <paramref name="replacement"/>.
    ///
    /// Does <i>not</i> change references to <paramref name="self"/>.
    /// </summary>
    /// <param name="self">Base node, must have a parent.</param>
    /// <param name="replacement">Node that will replace <paramref name="self"/> in <paramref name="self"/>'s parent.</param>
    /// <typeparam name="T">Type of <paramref name="replacement"/>.</typeparam>
    /// <returns><paramref name="replacement"/></returns>
    /// <exception cref="TreeShapeException">If <paramref name="self"/> has no parent.</exception>
    public static T ReplaceWith<T>(this INode self, T replacement) where T : INode
    {
        INode? parent = self.GetParent();
        if (parent == null)
            throw new TreeShapeException(self, "Cannot replace a node with no parent");

        Containment containment = parent.GetContainmentOf(self)!;

        if (containment.Multiple)
        {
            var value = parent.Get(containment);
            if (value is not IEnumerable enumerable)
                // should not happen
                throw new TreeShapeException(self, "Multiple containment does not yield enumerable");

            var nodes = enumerable.Cast<INode>().ToList();
            var index = nodes.IndexOf(self);
            if (index < 0)
                // should not happen
                throw new TreeShapeException(self, "Node not contained in its parent");

            nodes.Insert(index, replacement);
            nodes.Remove(self);
            parent.Set(containment, nodes);
        } else
        {
            parent.Set(containment, replacement);
        }

        return replacement;
    }

    /// <summary>
    /// Enumerates all direct and indirect children of <paramref name="self"/>.
    /// Optionally includes <paramref name="self"/> and/or directly and indirectly contained annotations.
    /// </summary>
    /// <param name="self">Base node to find descendants of.</param>
    /// <param name="includeSelf">If true, the result includes <paramref name="self"/>.</param>
    /// <param name="includeAnnotations">If true, the result includes directly and indirectly contained annotations.</param>
    /// <returns>All directly and indirectly contained nodes of <paramref name="self"/>.</returns>
    public static IEnumerable<INode> Descendants(this INode self, bool includeSelf = false,
        bool includeAnnotations = false) =>
        Descendants<INode>(self, includeSelf, includeAnnotations);

    /// <summary>
    /// Enumerates all direct children of <paramref name="self"/>.
    /// Optionally includes <paramref name="self"/> and/or directly contained annotations.
    /// </summary>
    /// <param name="self">Base node to find children of.</param>
    /// <param name="includeSelf">If true, the result includes <paramref name="self"/>.</param>
    /// <param name="includeAnnotations">If true, the result includes directly contained annotations.</param>
    /// <returns>All directly contained nodes of <paramref name="self"/>.</returns>
    public static IEnumerable<INode> Children(this INode self, bool includeSelf = false,
        bool includeAnnotations = false) =>
        Children<INode>(self, includeSelf, includeAnnotations);


    /// <summary>
    /// Returns the first ancestor of <paramref name="self"/> that matches with type <typeparamref name="T"/>
    /// Optionally includes <paramref name="self"/>.
    /// </summary>
    /// <param name="self">Base node to find ancestor of.</param>
    /// <param name="includeSelf">If true, the result includes <paramref name="self"/>.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>The first ancestor of <paramref name="self"/> that matches with type <typeparamref name="T"/> or null </returns>
    public static T? Ancestor<T>(this INode self, bool includeSelf = false) where T : INode =>
        self.Ancestors(includeSelf)
            .OfType<T>()
            .FirstOrDefault();

    /// <summary>
    /// Enumerates all direct and indirect parents of <paramref name="self"/>.
    /// Optionally includes <paramref name="self"/>.
    /// </summary>
    /// <param name="self">Base node to find ancestors of.</param>
    /// <param name="includeSelf">If true, the result includes <paramref name="self"/>.</param>
    /// <returns>All direct and indirect parents of <paramref name="self"/>.</returns>
    public static IEnumerable<INode> Ancestors(this INode self, bool includeSelf = false) =>
        Ancestors<INode>(self, includeSelf);

    private static List<INode> GetContainmentNodes(INode self)
    {
        INode? parent = self.GetParent();
        if (parent == null)
            throw new TreeShapeException(self, "Cannot get siblings of a node with no parent");

        Containment containment = parent.GetContainmentOf(self)!;
        if (!containment.Multiple)
            throw new TreeShapeException(self, "A single containment does not have siblings");

        var value = parent.Get(containment);
        if (value is not IEnumerable enumerable)
            // should not happen
            throw new TreeShapeException(self, "A single containment does not have siblings");

        return [..containment.AsNodes<INode>(enumerable)];
    }

    internal static IEnumerable<T> Descendants<T>(T self, bool includeSelf = false,
        bool includeAnnotations = false) where T : class, IReadableNode
    {
        var result = Children(self, false, includeAnnotations)
            .SelectMany(child => Descendants(child, includeSelf: true, includeAnnotations: includeAnnotations));

        if (includeSelf)
            result = result.Prepend(self);

        return result;
    }

    internal static IEnumerable<T> Children<T>(T self, bool includeSelf = false,
        bool includeAnnotations = false) where T : class, IReadableNode
    {
        var result = self
            .CollectAllSetFeatures()
            .OfType<Containment>()
            .SelectMany<Containment, T>(containment => containment.AsNodes<T>(self.Get(containment)));

        if (includeAnnotations)
            result = result.Concat(self.GetAnnotations().Cast<T>());

        if (includeSelf)
            result = result.Prepend(self);

        return result;
    }

    internal static IEnumerable<T> Ancestors<T>(T self, bool includeSelf = false) where T : class, IReadableNode
    {
        var result = Enumerable.Empty<T>();
        var parent = (T?)self.GetParent();
        if (parent != null)
            result = Ancestors(parent, true);

        if (includeSelf)
            result = result.Prepend(self);

        return result;
    }
}