// // Copyright 2024 TRUMPF Laser SE and other contributors
// // 
// // Licensed under the Apache License, Version 2.0 (the "License");
// // you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at
// // 
// //     http://www.apache.org/licenses/LICENSE-2.0
// // 
// // Unless required by applicable law or agreed to in writing, software
// // distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and
// // limitations under the License.
// // 
// // SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// // SPDX-License-Identifier: Apache-2.0
//
// namespace LionWeb_CSharp_Test.tests.compressed;
//
// using LionWeb.Core.M1;
//
// [TestClass]
// public class CompressedIdTests
// {
//     [TestMethod]
//     public void Equals()
//     {
//         var left = CompressedId.Create("a");
//         var right = CompressedId.Create("a");
//         var actual = left.Equals(right);
//         Assert.IsTrue(actual);
//     }
//
//     [TestMethod]
//     public void NotEquals()
//     {
//         var left = CompressedId.Create("a");
//         var right = CompressedId.Create("b");
//         var actual = left.Equals(right);
//         Assert.IsFalse(actual);
//     }
//
//     [TestMethod]
//     public void EqualSigns()
//     {
//         var left = CompressedId.Create("a");
//         var right = CompressedId.Create("a");
//         var actual = left == right;
//         Assert.IsTrue(actual);
//     }
//
//     [TestMethod]
//     public void NotEqualSigns()
//     {
//         var left = CompressedId.Create("a");
//         var right = CompressedId.Create("a");
//         var actual = left != right;
//         Assert.IsTrue(actual);
//     }
//
//     [TestMethod]
//     public void HashCode()
//     {
//         Assert.AreEqual(CompressedId.Create("a").GetHashCode(), CompressedId.Create("a").GetHashCode());
//     }
//     
//     [TestMethod]
//     public void NotHashCode()
//     {
//         Assert.AreNotEqual(CompressedId.Create("a").GetHashCode(), CompressedId.Create("b").GetHashCode());
//     }
// }
//
// [TestClass]
// public class DefaultIdTests
// {
//     [TestMethod]
//     public void Equals_Default()
//     {
//         var left = DefaultId.Create("a");
//         var right = DefaultId.Create("a");
//         var actual = left.Equals(right);
//         Assert.IsTrue(actual);
//     }
//
//     [TestMethod]
//     public void NotEquals_Default()
//     {
//         var left = DefaultId.Create("a");
//         var right = DefaultId.Create("b");
//         var actual = left.Equals(right);
//         Assert.IsFalse(actual);
//     }
//
//     [TestMethod]
//     public void NotEqualSigns_Default()
//     {
//         var left = DefaultId.Create("a");
//         var right = DefaultId.Create("b");
//         var actual = left != right;
//         Assert.IsTrue(actual);
//     }
//
//     [TestMethod]
//     public void HashCode_Default()
//     {
//         Assert.AreEqual(DefaultId.Create("a").GetHashCode(), DefaultId.Create("a").GetHashCode());
//     }
//
//     [TestMethod]
//     public void NotHashCode_Default()
//     {
//         Assert.AreNotEqual(DefaultId.Create("a").GetHashCode(), DefaultId.Create("b").GetHashCode());
//     }
// }
//
// [TestClass]
// public class StructIdTests
// {
//     [TestMethod]
//     public void Equals_Struct()
//     {
//         var left = StructId.Create("a");
//         var right = StructId.Create("a");
//         var actual = left.Equals(right);
//         Assert.IsTrue(actual);
//     }
//
//     [TestMethod]
//     public void NotEquals_Struct()
//     {
//         var left = StructId.Create("a");
//         var right = StructId.Create("b");
//         var actual = left.Equals(right);
//         Assert.IsFalse(actual);
//     }
//
//     [TestMethod]
//     public void HashCode_Struct()
//     {
//         Assert.AreEqual(StructId.Create("a").GetHashCode(), StructId.Create("a").GetHashCode());
//     }
//
//     [TestMethod]
//     public void NotHashCode_Struct()
//     {
//         Assert.AreNotEqual(StructId.Create("a").GetHashCode(), StructId.Create("b").GetHashCode());
//     }
// }
//
// [TestClass]
// public class CustomStructIdTests
// {
//     [TestMethod]
//     public void Equals_CustomStruct()
//     {
//         var left = CustomStructId.Create("a");
//         var right = CustomStructId.Create("a");
//         var actual = left.Equals(right);
//         Assert.IsTrue(actual);
//     }
//
//     [TestMethod]
//     public void NotEquals_CustomStruct()
//     {
//         var left = CustomStructId.Create("a");
//         var right = CustomStructId.Create("b");
//         var actual = left.Equals(right);
//         Assert.IsFalse(actual);
//     }
//
//     [TestMethod]
//     public void HashCode_CustomStruct()
//     {
//         Assert.AreEqual(CustomStructId.Create("a").GetHashCode(), CustomStructId.Create("a").GetHashCode());
//     }
//
//     [TestMethod]
//     public void NotHashCode_CustomStruct()
//     {
//         Assert.AreNotEqual(CustomStructId.Create("a").GetHashCode(), CustomStructId.Create("b").GetHashCode());
//     }
// }