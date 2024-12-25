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

namespace LionWeb.Core.M2;

using M1;
using M3;

/// <summary>
/// Callbacks to customize a <see cref="ILanguageDeserializer"/>'s behaviour in non-regular situations.
///
/// <para>
/// Each method of this interface is one callback. It should provide all relevant information as parameters.
/// If the method returns non-null, the returned value is used to <i>heal</i> the issue; otherwise, the offender is skipped (if possible).
/// </para>
/// </summary>
public interface ILanguageDeserializerHandler : IDeserializerHandler
{
    /// <summary>
    /// Cannot install containments into <paramref name="node"/>. 
    /// </summary>
    /// <param name="node">Node that cannot receive new containments.</param>
    void InvalidContainment(IReadableNode node);

    /// <summary>
    /// Cannot install annotations into <paramref name="parent"/>.
    /// </summary>
    /// <param name="annotation">Annotation we want to add to <paramref name="parent"/>.</param>
    /// <param name="parent">Node that cannot receive new annotations.</param>
    void InvalidAnnotationParent(IReadableNode annotation, IReadableNode? parent);

    /// <summary>
    /// <paramref name="structuredDataType"/> contains itself directly or indirectly.
    /// </summary>
    /// <param name="structuredDataType">Offending Structured DataType.</param>
    /// <param name="owners">All StructuredDataTypes that form a containment cycle.</param>
    void CircularStructuredDataType(StructuredDataType structuredDataType, ISet<StructuredDataType> owners);
}