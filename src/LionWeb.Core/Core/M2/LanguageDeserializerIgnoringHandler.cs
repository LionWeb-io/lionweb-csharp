﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.M2;

using M1;
using M3;

/// <inheritdoc cref="DeserializerIgnoringHandler" />
public class LanguageDeserializerIgnoringHandler : DeserializerIgnoringHandler, ILanguageDeserializerHandler
{
    /// <inheritdoc />
    public void InvalidContainment(IReadableNode node) =>
        LogMessage($"installing containments in node of meta-concept {node.GetType().Name} not implemented");

    /// <inheritdoc />
    public void InvalidAnnotationParent(IReadableNode annotation, IReadableNode? parent) =>
        LogMessage($"Cannot attach annotation {annotation} to its parent with id={parent?.GetId()}.");

    /// <inheritdoc />
    public void CircularStructuredDataType(StructuredDataType structuredDataType, ISet<StructuredDataType> owners) => 
    LogMessage($"StructuredDataType {structuredDataType.Name} not supported: contains itself via {owners}");
}