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

namespace LionWeb.Protocol.Delta.Test.Repository;

using Client;
using Core;
using Core.M1;
using Core.M3;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using Core.Utilities;
using Delta.Client;
using Delta.Repository;

[TestClass]
public abstract class RepositoryTestsBase
{
    protected const RepositoryId RepoId = "myRepo";
    protected const int DefaultDepthLimit = 7;
    
    private readonly IForest _repositoryForest;
    protected readonly LionWebTestRepository _repository;
    private readonly DeltaRepositoryConnector _deltaRepositoryConnector;

    protected readonly IForest _aForest;
    protected readonly LionWebTestClient _aClient;

    protected readonly IForest _bForest;
    protected readonly LionWebTestClient _bClient;

    protected readonly TestPartition _partition;
    protected readonly LinkTestConcept _linkLevel1B;
    protected readonly TestAnnotation _annLevel2Ba;
    protected readonly LinkTestConcept _linkLevel3Baa;
    protected readonly TestAnnotation _annLevel4Baaa;
    protected readonly LinkTestConcept _linkLevel1A;
    protected readonly LinkTestConcept _linkLevel2Ac;
    protected readonly LinkTestConcept _linkLevel2Ab;
    protected readonly TestAnnotation _annLevel3Aba;
    protected readonly LinkTestConcept _linkLevel2Aa;
    protected readonly LinkTestConcept _linkLevel3Aab;
    protected readonly LinkTestConcept _linkLevel3Aaa;
    protected readonly TestAnnotation _annLevel4Aaaa;
    protected readonly DataTypeTestConcept _dataLevel5Aaaaa;

    private TestDeltaClientConnector _aConnector;
    private TestDeltaClientConnector _bConnector;
    private readonly List<Language> _languages;
    private readonly IVersion2024_1 _lionWebVersion;

    public RepositoryTestsBase()
    {
        _lionWebVersion = LionWebVersions.v2024_1;
        _languages = [TestLanguageLanguage.Instance];

        _repositoryForest = new Forest();
        _deltaRepositoryConnector = new(_lionWebVersion);

        _repository = new LionWebTestRepository(_lionWebVersion, _languages, "repository", _repositoryForest,
            _deltaRepositoryConnector, Log);

        _aClient = CreateClient("A", out _aForest, out _aConnector, Log);
        _bClient = CreateClient("B", out _bForest, out _bConnector, Log);
        
        _dataLevel5Aaaaa = new DataTypeTestConcept("dataLevel5_A_A_A_A_A");
        _annLevel4Aaaa = new TestAnnotation("annLevel4_A_A_A_A") { Containment = _dataLevel5Aaaaa };
        _linkLevel3Aaa = new LinkTestConcept("linkLevel3_A_A_A").WithAnnotation(_annLevel4Aaaa);
        _linkLevel3Aab = new LinkTestConcept("linkLevel3_A_A_B");
        _linkLevel2Aa = new LinkTestConcept("linkLevel2_A_A")
        {
            Containment_0_n =
            [
                _linkLevel3Aaa,
                _linkLevel3Aab
            ]
        };
        _annLevel3Aba = new TestAnnotation("annLevel3_A_B_A");
        _linkLevel2Ab = new LinkTestConcept("linkLevel2_A_B").WithAnnotation(_annLevel3Aba);
        _linkLevel2Ac = new LinkTestConcept("linkLevel2_A_C");
        _linkLevel1A = new LinkTestConcept("linkLevel1_A")
        {
            Containment_0_1 = _linkLevel2Aa,
            Containment_1_n =
            [
                _linkLevel2Ab,
                _linkLevel2Ac,
            ]
        };
        _annLevel4Baaa = new TestAnnotation("annLevel4_B_A_A_A");
        _linkLevel3Baa = new LinkTestConcept("linkLevel3_B_A_A").WithAnnotation(_annLevel4Baaa);
        _annLevel2Ba = new TestAnnotation("annLevel2_B_A") { Containment = _linkLevel3Baa };
        _linkLevel1B = new LinkTestConcept("linkLevel1_B").WithAnnotation(_annLevel2Ba);
        _partition = new TestPartition("partition")
        {
            Links =
            [
                _linkLevel1A,
                _linkLevel1B
            ]
        };
    }

    private LionWebTestClient CreateClient(string name, out IForest forest, out TestDeltaClientConnector connector,
        Action<string> logger)
    {
        var clientId = $"{name}ClientId";
        forest = new Forest();
        connector = new TestDeltaClientConnector(_lionWebVersion);
        var client = new LionWebTestClient(_lionWebVersion, _languages, name, forest, connector, logger)
        {
            ClientId = clientId
        };
        connector.Connect(clientId, _deltaRepositoryConnector);
        return client;
    }


    protected void AssertEquals(IReadableNode? a, IReadableNode? b) =>
        AssertEquals([a], [b]);

    protected void AssertEquals(IEnumerable<IReadableNode?> a, IEnumerable<IReadableNode?> b)
    {
        List<IDifference> differences = new Comparer(a.ToList(), b.ToList()).Compare().ToList();
        Assert.IsTrue(differences.Count == 0,
            differences.DescribeAll(new() { LeftDescription = "a", RightDescription = "b" }));
    }
    
    protected static void AssertNodesPresent(List<IReadableNode> expected, List<IPartitionInstance> actualPartitions) =>
        CollectionAssert.AreEquivalent(
            expected.Select(n => n.GetId()).ToList(),
            actualPartitions.SelectMany(p => M1Extensions.Descendants<IReadableNode>(p, true, true)).Select(n => n.GetId()).ToList()
        );

    protected void AssertNoExceptions(List<Exception> exceptions) =>
        Assert.AreEqual(0, exceptions.Count, string.Join(Environment.NewLine, exceptions));

    protected void WaitForReceived(int numberOfMessages)
    {
        _aClient.WaitForReceived(numberOfMessages);
        _bClient.WaitForReceived(numberOfMessages);
    }

    private static void Log(string message) =>
        Console.WriteLine(message);

    protected void AssertNoExceptions()
    {
        AssertNoExceptions(_repository.Exceptions);
        AssertNoExceptions(_aClient.Exceptions);
        AssertNoExceptions(_bClient.Exceptions);
    }
}

public abstract class RepositoryTestNoExceptionsBase : RepositoryTestsBase
{
    [TestCleanup]
    public new void AssertNoExceptions() => 
        base.AssertNoExceptions();
}