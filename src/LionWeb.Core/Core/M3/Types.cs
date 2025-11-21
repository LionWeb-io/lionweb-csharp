// Copyright 2024 TRUMPF Laser SE and other contributors
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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.M3;

// The types here implement the LionCore M3.

/// Represents a relation between a containing <see cref="Classifier"/> and a contained <see cref="Classifier"/>.
public interface Containment : Link;

/// Represents a relation between a referring <see cref="Classifier"/> and referred <see cref="Classifier"/>.
public interface Reference : Link;

/// A LanguageEntity is an entity with an identity directly contained in a <see cref="Language"/>.
public interface LanguageEntity : IKeyed;

/// <summary>
/// A type of value which has no relevant identity in the context of a model.
/// </summary>
/// <remarks>
/// In official LionWeb, the correct name is <tt>DataType</tt> (uppercase T).
/// We keep the lowercase version for backwards compatibility.
/// </remarks>
public interface Datatype : LanguageEntity;

/// This represents an arbitrary primitive value.
public interface PrimitiveType : Datatype;

/// One of the possible values of an <see cref="Enumeration"/>.
public interface EnumerationLiteral : IKeyed;