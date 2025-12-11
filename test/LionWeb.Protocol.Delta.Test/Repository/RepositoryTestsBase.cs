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
using Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using Core.Utilities;
using Delta.Client;
using Delta.Repository;

[TestClass]
public abstract class RepositoryTestsBase
{
    protected const RepositoryId RepoId = "myRepo";
    
    private readonly IForest _repositoryForest;
    protected readonly LionWebTestRepository _repository;
    private readonly DeltaRepositoryConnector _deltaRepositoryConnector;

    protected readonly IForest _aForest;
    protected readonly LionWebTestClient _aClient;

    protected readonly IForest _bForest;
    protected readonly LionWebTestClient _bClient;
    private TestDeltaClientConnector _aConnector;
    private TestDeltaClientConnector _bConnector;
    private readonly List<Language> _languages;
    private readonly IVersion2024_1 _lionWebVersion;

    public RepositoryTestsBase()
    {
        _lionWebVersion = LionWebVersions.v2024_1;
        _languages = [ShapesLanguage.Instance, TestLanguageLanguage.Instance];

        _repositoryForest = new Forest();
        _deltaRepositoryConnector = new(_lionWebVersion);
        _repository = new LionWebTestRepository(_lionWebVersion, _languages, "repository", _repositoryForest,
            _deltaRepositoryConnector);

        _aClient = CreateClient("A", out _aForest, out _aConnector);
        _bClient = CreateClient("B", out _bForest, out _bConnector);
    }

    private LionWebTestClient CreateClient(string name, out IForest forest, out TestDeltaClientConnector connector)
    {
        var clientId = $"{name}ClientId";
        forest = new Forest();
        connector = new TestDeltaClientConnector(_lionWebVersion);
        var client = new LionWebTestClient(_lionWebVersion, _languages, name, forest, connector)
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

    protected void AssertNoExceptions(List<Exception> exceptions) =>
        Assert.AreEqual(0, exceptions.Count, string.Join(Environment.NewLine, exceptions));

    protected void WaitForReceived(int numberOfMessages)
    {
        _aClient.WaitForReceived(numberOfMessages);
        _bClient.WaitForReceived(numberOfMessages);
    }
}

public abstract class RepositoryTestNoExceptionsBase : RepositoryTestsBase
{
    [TestCleanup]
    public void AssertNoExceptions()
    {
        AssertNoExceptions(_repository.Exceptions);
        AssertNoExceptions(_aClient.Exceptions);
        AssertNoExceptions(_bClient.Exceptions);
    }
}