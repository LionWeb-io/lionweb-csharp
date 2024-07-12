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

namespace LionWeb.Core.M1;

using System.Security.Cryptography;
using System.Text;

public class DuplicateIdChecker
{
    private class HashByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[]? x, byte[]? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            
            if (x == null || y == null)
                return false;

            return ((ReadOnlySpan<byte>)x).SequenceEqual((ReadOnlySpan<byte>)y);
        }

        public int GetHashCode(byte[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return BitConverter.ToInt32(obj, 0);
        }
    }

    private readonly HashSet<byte[]> _knownIds = new(new HashByteArrayComparer());
    private readonly SHA1 _sha1 = SHA1.Create();

    public bool IsIdDuplicate(string id)
    {
        _sha1.Initialize();
        var idHash = _sha1.ComputeHash(Encoding.ASCII.GetBytes(id));
        return !_knownIds.Add(idHash);
    }
}