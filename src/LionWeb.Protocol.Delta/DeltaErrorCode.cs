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

namespace LionWeb.Protocol.Delta;

using Message;
using Message.Event;
using Message.Query;

public enum DeltaErrorCode
{
    InvalidNodeType,
    UnknownNodeId,
    NotSignedOn,
    AlreadySignedOn,
    NotCurrentSequenceNumber,
    UnknownPartition,
    NotSubscribed
}

public static class DeltaErrorCodeExtensions
{
    static string GetMessage(this DeltaErrorCode errorCode, params object[] args) => string.Format(errorCode switch
    {
        DeltaErrorCode.InvalidNodeType => "Invalid node type: {0} is not a partition",
        DeltaErrorCode.UnknownNodeId => "Unknown node id {0}",
        DeltaErrorCode.NotSignedOn => "Not signed on",
        DeltaErrorCode.AlreadySignedOn => "Already signed on",
        DeltaErrorCode.NotCurrentSequenceNumber => "Last sent sequence number is {0} vs. last received {1}",
        DeltaErrorCode.UnknownPartition => "Unknown partition '{0}'",
        DeltaErrorCode.NotSubscribed => "Not subscribed to partition '{0}'",
    }, args);

    public static Error AsError(this DeltaErrorCode errorCode, CommandSource[]? originCommands,
        ProtocolMessage[]? protocolMessages, params object[] args) =>
        new Error(errorCode.ToString(), GetMessage(errorCode, args), originCommands, protocolMessages);

    public static ErrorResponse AsErrorResponse(this DeltaErrorCode errorCode, QueryId queryId,
        ProtocolMessage[]? protocolMessages, params object[] args) =>
        new ErrorResponse(errorCode.ToString(), GetMessage(errorCode, args), queryId, protocolMessages);
}