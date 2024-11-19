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

namespace LionWeb_CSharp_Test.languages;

using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;

public static class InvalidLanguage
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private static readonly IBuiltInsLanguage _builtIns = _lionWebVersion.BuiltIns;

    public static readonly DynamicLanguage Language = new("id-Invalid", _lionWebVersion)
    {
        Key = "key-Invalid", Name = "InvalidLanguage", Version = "a-_1"
    };

    static InvalidLanguage()
    {
        var IA = Language.Interface("id-IA", "key-IA", "IA");
        var IB = Language.Interface("id-IB", "key-IB", "IB");
        var IC = Language.Interface("id-IC", "key-IC", "IC");
        var ID = Language.Interface("id-ID", "key-ID", "ID");
        var IE = Language.Interface("id-IE", "key-IE", "IE");
        var IF = Language.Interface("id-IF", "key-IF", "IF");
        var IG = Language.Interface("id-IG", "key-IG", "IG");
        var IH = Language.Interface("id-IH", "key-IH", "IH");
        var II = Language.Interface("id-II", "key-II", "II");
        var IJ = Language.Interface("id-IJ", "key-IJ", "IJ");
        var IK = Language.Interface("id-IK", "key-IK", "IK");
        var IL = Language.Interface("id-IL", "key-IL", "IL");

        var ValidI0 = Language.Interface("id-ValidI0", "key-ValidI0", "ValidI0");
        var ValidI1 = Language.Interface("id-ValidI1", "key-ValidI1", "ValidI1");
        var ValidI2 = Language.Interface("id-ValidI2", "key-ValidI2", "ValidI2");
        var ValidI3 = Language.Interface("id-ValidI3", "key-ValidI3", "ValidI3");
        var ValidI4 = Language.Interface("id-ValidI4", "key-ValidI4", "ValidI4");
        var ValidI5 = Language.Interface("id-ValidI5", "key-ValidI5", "ValidI5");
        var ValidI6 = Language.Interface("id-ValidI6", "key-ValidI6", "ValidI6");
        var ValidI7 = Language.Interface("id-ValidI7", "key-ValidI7", "ValidI7");
        var ValidI8 = Language.Interface("id-ValidI8", "key-ValidI8", "ValidI8");

        var CA = Language.Concept("id-CA", "key-CA", "CA");
        var CB = Language.Concept("id-CB", "key-CB", "CB");
        var CC = Language.Concept("id-CC", "key-CC", "CC");
        var CD = Language.Concept("id-CD", "key-CD", "CD");
        var CE = Language.Concept("id-CE", "key-CE", "CE");
        var CF = Language.Concept("id-CF", "key-CF", "CF");
        var ValidC0 = Language.Concept("id-ValidC0", "key-ValidC0", "ValidC0");
        var ValidC1 = Language.Concept("id-ValidC1", "key-ValidC1", "ValidC1");
        var ValidC2 = Language.Concept("id-ValidC2", "key-ValidC2", "ValidC2");
        var ValidC3 = Language.Concept("id-ValidC3", "key-ValidC3", "ValidC3");
        var ValidC4 = Language.Concept("id-ValidC4", "key-ValidC4", "ValidC4");
        var C5 = Language.Concept("id-ValidC5", "key-ValidC5", "ValidC5");

        var AA = Language.Annotation("id-AA", "key-AA", "AA");
        var AB = Language.Annotation("id-AB", "key-AB", "AB");
        var AC = Language.Annotation("id-AC", "key-AC", "AC");
        var AD = Language.Annotation("id-AD", "key-AD", "AD");
        var AE = Language.Annotation("id-AE", "key-AE", "AE");
        var AF = Language.Annotation("id-AF", "key-AF", "AF");
        var ValidA0 = Language.Annotation("id-ValidA0", "key-ValidA0", "ValidA0");
        var ValidA1 = Language.Annotation("id-ValidA1", "key-ValidA1", "ValidA1");
        var ValidA2 = Language.Annotation("id-ValidA2", "key-ValidA2", "ValidA2");
        var ValidA3 = Language.Annotation("id-ValidA3", "key-ValidA3", "ValidA3");
        var ValidA4 = Language.Annotation("id-ValidA4", "key-ValidA4", "ValidA4");
        var A5 = Language.Annotation("id-ValidA5", "key-ValidA5", "ValidA5");

        // direct interface circle:
        // A <-> B
        IA.Extending(IB);
        IB.Extending(IA);

        // indirect interface circle:
        // C -> D -> E -> C
        IC.Extending(ID);
        ID.Extending(IE);
        IE.Extending(IC);

        IC.Property("id-IC-propA", "key-IC-propA", "propA").OfType(_builtIns.String);
        ID.Property("id-ID-propA", "key-ID-propA", "propA").OfType(_builtIns.String);
        IE.Property("id-IE-propA", "key-IE-propA", "propA").OfType(_builtIns.Boolean);

        // secondary direct interface circle:
        // F -> G
        // H -> G -> H
        IF.Extending(IG);
        IH.Extending(IG);
        IG.Extending(IH);

        // diamond interface circle:
        // I -> J -> K -> I
        // I -> L -> K
        II.Extending(IJ);
        IJ.Extending(IK);
        II.Extending(IL);
        IL.Extending(IK);
        IK.Extending(II);

        // valid interface diamond:
        // 0 -> 1 -> 2
        // 0 -> 3 -> 2
        ValidI0.Extending(ValidI1);
        ValidI1.Extending(ValidI2);
        ValidI0.Extending(ValidI3);
        ValidI3.Extending(ValidI2);

        // valid interface with two generalizations:
        // 7 -> 5
        // 7 -> 6
        ValidI7.Extending(ValidI5);
        ValidI7.Extending(ValidI6);

        // valid interface line:
        // 8 -> 7 -> 5
        ValidI8.Extending(ValidI7);

        // direct concept circle:
        // A <-> B
        CA.Extending(CB);
        CB.Extending(CA);

        // indirect concept circle:
        // C -> D -> E -> C
        CC.Extending(CD);
        CD.Extending(CE);
        CE.Extending(CC);

        CC.Reference("id-CC-refA", "key-CC-refA", "refA").OfType(CC);
        CD.Reference("id-CD-refA", "key-CD-refA", "refA").OfType(CC);
        CE.Reference("id-CE-refA", "key-CE-refA", "refA").OfType(CA);

        // concept tapping into interface diamond circle:
        // CF -> IJ -> IK -> IL
        // CF -> IL -> IK
        CF.Implementing(IJ);
        CF.Implementing(IL);

        // valid concept line tapping into interface diamond:
        // C0 -> C1 -> C2 -> I0 -> I1 -> I2
        // C1 -> I3 -> I2
        ValidC0.Extending(ValidC1);
        ValidC1.Extending(ValidC2);
        ValidC2.Implementing(ValidI0);
        ValidC1.Implementing(ValidI3);

        // Valid concept with multiple interfaces:
        // C4 -> I5
        // C4 -> I6
        ValidC4.Implementing(ValidI5);
        ValidC4.Implementing(ValidI6);

        // Kind-of-valid concept with multiple interfaces, implementing the same interface twice:
        // C5 -> I5
        // C5 -> I6
        // C5 -> I5
        C5.Implementing(ValidI5);
        C5.Implementing(ValidI6);
        C5.Implementing(ValidI5);

        // direct annotation circle:
        // A <-> B
        AA.Extending(AB);
        AB.Extending(AA);

        // indirect annotation circle:
        // C -> D -> E -> C
        AC.Extending(AD);
        AD.Extending(AE);
        AE.Extending(AC);

        AC.Containment("id-AC-contA", "key-AC-contA", "contA").OfType(CC);
        AD.Containment("id-AD-contA", "key-AD-contA", "contA").OfType(CC);
        AE.Containment("id-AE-contA", "key-AE-contA", "contA").OfType(CA);

        // annotation tapping into interface diamond circle:
        // AF -> IJ -> IK -> IL
        // AF -> IL -> IK
        AF.Implementing(IJ);
        AF.Implementing(IL);

        // valid annotation line tapping into interface diamond:
        // A0 -> A1 -> A2 -> I0 -> I1 -> I2
        // A1 -> I3 -> I2
        ValidA0.Extending(ValidA1);
        ValidA1.Extending(ValidA2);
        ValidA2.Implementing(ValidI0);
        ValidA1.Implementing(ValidI3);

        // Valid annotation with multiple interfaces:
        // A4 -> I5
        // A4 -> I6
        ValidA4.Implementing(ValidI5);
        ValidA4.Implementing(ValidI6);

        // Kind-of-valid annotation with multiple interfaces, implementing the same interface twice:
        // A5 -> I5
        // A5 -> I6
        // A5 -> I5
        A5.Implementing(ValidI5);
        A5.Implementing(ValidI6);
        A5.Implementing(ValidI5);
    }
}