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

namespace LionWeb.Core.M1.Event;

public interface IProcessor
{
    public string ProcessorId { get; }
    
    public static void Forward<TReceiveFrom, TForward, TSendFrom>(
        IProcessor<TReceiveFrom, TForward> from,
        IProcessor<TForward, TSendFrom> to)
    {
        from.Subscribe(to);
        List<IProcessor> alreadyPrinted = [];
        // from.PrintAllReceivers(alreadyPrinted);
        // Console.WriteLine();

        if (alreadyPrinted.GroupBy(it => it).Any(it => it.Count() > 1))
        {
            // throw new ArgumentException($"Recursion between {from} and {to}");
        }
    }

    public void PrintAllReceivers(List<IProcessor> alreadyPrinted, string indent = "");

    protected static bool RecursionDetected(IProcessor self, List<IProcessor> alreadyPrinted, string indent)
    {
        try
        {
            if (!alreadyPrinted.Contains(self))
                return false;

            Console.WriteLine($"{indent}Recursion ^^");
            return true;

        } finally
        {
            alreadyPrinted.Add(self);
        }
    }
}

public interface IProcessor<in TReceive, in TSend> : IProcessor
{
    public bool CanReceive(params Type[] messageTypes);
    
    public void Receive(TReceive message);
    protected void Send(TSend message);

    
    internal void Subscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver);
    
    internal void Unsubscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver);
}