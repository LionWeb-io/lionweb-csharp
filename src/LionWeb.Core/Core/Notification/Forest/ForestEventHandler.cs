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

<<<<<<<< HEAD:src/LionWeb.Protocol.Delta/Client/ILionWebClient.cs
namespace LionWeb.Protocol.Delta.Client;

public interface ILionWebClient
{
    private const string _magenta = "\x1b[95m";
    private const string _bold = "\x1b[1m";
    private const string _unbold = "\x1b[22m";
    private const string _defaultColor = "\x1b[39m";

    public const string HeaderColor_Start = _magenta + _bold;
    public const string HeaderColor_End = _unbold + _defaultColor;
}
========
namespace LionWeb.Core.Notification.Forest;

/// Forwards <see cref="IForestCommander"/> commands to <see cref="IForestPublisher"/> events.
/// <param name="sender">Optional sender of the events.</param>
public class ForestEventHandler(object? sender)
    : EventHandlerBase<IForestNotification>(sender), IForestPublisher, IForestCommander;
>>>>>>>> main:src/LionWeb.Core/Core/Notification/Forest/ForestEventHandler.cs
