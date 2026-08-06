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

namespace LionWeb.Protocol.Delta.Test.Repository;

using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using Message;
using Message.Event;

[TestClass]
public class DeltaRepositoryConnectorEventSequenceNumberTests : RepositoryTestsBase
{
    private readonly List<EventSequenceNumber> _aSequenceNumbers;
    private readonly List<EventSequenceNumber> _bSequenceNumbers;

    public DeltaRepositoryConnectorEventSequenceNumberTests()
    {
        _aSequenceNumbers = [];
        _bSequenceNumbers = [];

        _aConnector.ReceivedFromRepository += RegisterSequenceNumber(_aSequenceNumbers);
        _bConnector.ReceivedFromRepository += RegisterSequenceNumber(_bSequenceNumbers);
    }

    [TestMethod]
    public async Task UniquePerClient()
    {
        await _aClient.SignOn(RepoId);
        await _aClient.SubscribeToChangingPartitions(true, true);
        await _bClient.SignOn(RepoId);
        await _bClient.SubscribeToChangingPartitions(true, true);

        _aForest.AddPartitions([new TestPartition("part")]);
        WaitForReceived(1);

        _bForest.RemovePartitions([.. _bForest.Partitions]);
        WaitForReceived(1);

        Assert.HasCount(2, _aSequenceNumbers);
        Assert.HasCount(2, _bSequenceNumbers);
    }

    [TestMethod]
    public async Task SequentialPerClient()
    {
        await _aClient.SignOn(RepoId);
        await _aClient.SubscribeToChangingPartitions(true, true);
        await _bClient.SignOn(RepoId);
        await _bClient.SubscribeToChangingPartitions(true, true);

        _aForest.AddPartitions([new TestPartition("part")]);
        WaitForReceived(1);

        _bForest.RemovePartitions([.. _bForest.Partitions]);
        WaitForReceived(1);

        CollectionAssert.AreEquivalent(new List<EventSequenceNumber> { 1, 2 }, _aSequenceNumbers);
        CollectionAssert.AreEquivalent(new List<EventSequenceNumber> { 1, 2 }, _bSequenceNumbers);
    }

    [TestMethod]
    public async Task Diverge()
    {
        await _aClient.SignOn(RepoId);
        await _aClient.SubscribeToChangingPartitions(true, false);
        await _bClient.SignOn(RepoId);
        await _bClient.SubscribeToChangingPartitions(true, false);

        _aForest.AddPartitions([new TestPartition("part0")]);
        WaitForReceived(1);

        await _bClient.UnsubscribeFromPartitionContents(_bForest.Partitions.First().GetId());
        _bClient.WaitForReceived(1);

        _aForest.RemovePartitions([.. _aForest.Partitions]);
        _aClient.WaitForReceived(1);

        _aForest.AddPartitions([new TestPartition("part1")]);
        WaitForReceived(1);

        CollectionAssert.AreEquivalent(new List<EventSequenceNumber> { 1, 2, 3 }, _aSequenceNumbers);
        CollectionAssert.AreEquivalent(new List<EventSequenceNumber> { 1, 2 }, _bSequenceNumbers);

        Assert.HasCount(2, _bForest.Partitions);
        Assert.AreEqual("part0", _bForest.Partitions.First().GetId());
        Assert.AreEqual("part1", _bForest.Partitions.Last().GetId());
    }

    [TestMethod]
    public async Task Composite()
    {
        await _aClient.SignOn(RepoId);
        await _aClient.SubscribeToChangingPartitions(true, true);
        await _bClient.SignOn(RepoId);
        await _bClient.SubscribeToChangingPartitions(true, true);

        _aClient.Compositor.Push();
        var aPartition = new TestPartition("part");
        _aForest.AddPartitions([aPartition]);
        aPartition.Name = "MyPartitionBefore";

        Assert.HasCount(0, _bForest.Partitions);
        _aClient.Compositor.Pop(true);
        WaitForReceived(1);
        
        aPartition.Name = "MyPartitionAfter";
        WaitForReceived(1);

        AssertEquals(aPartition, _bForest.Partitions.First());

        CollectionAssert.AreEquivalent(new List<EventSequenceNumber> { 1, 200, 300, 4 }, _aSequenceNumbers);
        CollectionAssert.AreEquivalent(new List<EventSequenceNumber> { 1, 200, 300, 4 }, _bSequenceNumbers);
    }

    private static EventHandler<IDeltaContent> RegisterSequenceNumber(List<EventSequenceNumber> storage) =>
        (_, dc) => RegisterSequenceNumber(storage, dc, 1);

    private static void RegisterSequenceNumber(List<EventSequenceNumber> storage, IDeltaContent dc, int factor)
    {
        if (dc is IDeltaEvent e)
        {
            var sequenceNumber = e.SequenceNumber * factor;
            Assert.DoesNotContain(sequenceNumber, storage);
            storage.Add(sequenceNumber);

            if (e is CompositeEvent ce)
            {
                foreach (var part in ce.Parts)
                {
                    RegisterSequenceNumber(storage, part, factor * 100);
                }
            }
        }
    }
}