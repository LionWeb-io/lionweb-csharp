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

namespace LionWeb.Core.Utilities;

using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Utility methods for working with LionWeb-compliant IDs.
/// </summary>
public static partial class IdUtils
{
    /// <returns>A generated, unique ID in base64url-format.</returns>
    public static NodeId NewId() =>
        EncodeBase64Url(Guid.NewGuid().ToByteArray());

    /// Encodes <paramref name="bytes"/> in base64url-format.
    public static NodeId EncodeBase64Url(byte[] bytes) =>
        ToUrlEncoding(Convert.ToBase64String(bytes));

    /// Encodes <paramref name="stringToEncode"/> in base64url-format.
    public static NodeId EncodeBase64Url(string stringToEncode) =>
        ToUrlEncoding(Convert.ToBase64String(Encoding.UTF8.GetBytes(stringToEncode)));

    private static NodeId ToUrlEncoding(string base64Encoded) =>
        base64Encoded
            .TrimEnd('=') // padding ='s can be safely removed
            .Replace("+", "-")
            .Replace("/", "_");


    /// Checks whether <paramref name="nodeId"/> is a valid node id.
    public static bool IsValid(NodeId nodeId) =>
        nodeId != null! && IdRegex().IsMatch(nodeId);

    [GeneratedRegex("^[a-zA-Z0-9_-]+$")]
    private static partial Regex IdRegex();
}