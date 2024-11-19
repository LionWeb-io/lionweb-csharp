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

namespace Examples.Shapes.Dynamic;

interface IVersion;

class V1 : IVersion;
class V2 : IVersion;

interface IUser<T> where T : IVersion
{
    string Name { get; }
    // DoIt<T> Del { get; init; }
    
    IDelegate<T> Deleg { get; init; }
    
    void Run();
}

class User<T> : IUser<T> where T : IVersion
{
    private readonly IDelegate<T> _deleg;

    public string Name { get; } = "me!";

    // public required DoIt<T> Del { get; init; }
    public IDelegate<T> Deleg
    {
        get => _deleg;
        init => _deleg = value.Register(this);
    }

    public void Run() => Deleg.DoIt();
}

delegate void DoIt<T>(IUser<T> user) where T : IVersion;

abstract class IDelegate<T> where T : IVersion
{
    protected IUser<T> _user { get; private set; }

    public IDelegate<T> Register(IUser<T> user)
    {
        _user = user;
        return this;
    }

    public abstract void DoIt();
}

class DelegateV1<T> : IDelegate<T> where T : V1
{
    public override void DoIt() => Console.WriteLine($"V1{_user.Name}");
}

class DelegateV2<T> : IDelegate<T> where T : IVersion
{
    public override void DoIt() => Console.WriteLine($"V2{_user.Name}");
}

static class Whatever
{
    public static IUser<T> CreateUser<T>(T version) where T : IVersion
    {
        // DoIt<V1> doIt = (user1) => Console.WriteLine($"V1{user1.Del}");
        //
        // DoIt<T> del = version switch
        // {
        //     V1 v1 => (user) =>
        //     {
        //         doIt.Invoke((IUser<V1>)user);
        //     },
        //     V2 v2 => (user) => ((DoIt<V2>)((user1) => Console.WriteLine($"V2{user1.Del}"))).Invoke((IUser<V2>)user),
        // };
        //
        // return new User<T>() { Del = del };
        
        IDelegate<T> deleg = version switch
        {
            V1 => new DelegateV1<T>(),
        }
    }
}
