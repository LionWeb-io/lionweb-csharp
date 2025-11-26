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

namespace LionWeb.Core.Test.NodeApi.Dynamic.StructuredDataType;

using M2;
using M3;

public abstract class StructuredDataTypeTestBase
{
    protected DynamicLanguage _sdtLang;

    [TestInitialize]
    public void LoadSdtLanguage()
    {
        _sdtLang = LanguagesUtils
            .LoadLanguages("LionWeb.Core.Test", "LionWeb.Core.Test.Languages.defChunks.sdtLang.json",
                LionWebVersions.v2024_1)
            .First();
    }

    protected DynamicNode NewSdtConcept(string id) =>
        _sdtLang.GetFactory().CreateNode(id, _sdtLang.ClassifierByKey("key-SDTConcept")) as DynamicNode ??
        throw new AssertFailedException();

    protected Property SdtConcept_decimal =>
        _sdtLang.ClassifierByKey("key-SDTConcept").FeatureByKey("key-SDTDecimalField") as Property ??
        throw new AssertFailedException();

    protected IStructuredDataTypeInstance NewDecimal(int dec, int frac)
    {
        var decSdt = Decimal_();
        var decValue = _sdtLang.GetFactory().CreateStructuredDataTypeInstance(decSdt,
            new FieldValues { { DecimalInt(), dec }, { DecimalFrac(), frac }, }
        );

        return decValue;
    }

    protected StructuredDataType Decimal_() => _sdtLang.StructuredDataTypeByKey("key-SDTDecimal");

    protected Field DecimalInt() => Decimal_().FieldByKey("key-SDTInt");
    protected Field DecimalFrac() => Decimal_().FieldByKey("key-SDTFrac");

    protected IStructuredDataTypeInstance NewDecimal()
    {
        var decSdt = Decimal_();
        var decValue = _sdtLang.GetFactory().CreateStructuredDataTypeInstance(decSdt,
            new FieldValues { }
        );

        return decValue;
    }

    protected IStructuredDataTypeInstance NewE() =>
        _sdtLang.GetFactory().CreateStructuredDataTypeInstance(E_(),
            new FieldValues { }
        );

    protected IStructuredDataTypeInstance NewE(IStructuredDataTypeInstance e2F) =>
        _sdtLang.GetFactory().CreateStructuredDataTypeInstance(E_(),
            new FieldValues { { E_e2f(), e2F }, }
        );

    protected IStructuredDataTypeInstance NewE(string name) =>
        _sdtLang.GetFactory().CreateStructuredDataTypeInstance(E_(),
            new FieldValues { { E_name(), name }, }
        );


    protected IStructuredDataTypeInstance NewE(IStructuredDataTypeInstance e2F, string? name) =>
        _sdtLang.GetFactory().CreateStructuredDataTypeInstance(E_(),
            new FieldValues { { E_e2f(), e2F }, { E_name(), name }, }
        );

    protected IStructuredDataTypeInstance NewF(string name) =>
        _sdtLang.GetFactory().CreateStructuredDataTypeInstance(F_(),
            new FieldValues { { F_name(), name }, }
        );

    protected StructuredDataType E_() => _sdtLang.StructuredDataTypeByKey("key-SDE");
    protected StructuredDataType F_() => _sdtLang.StructuredDataTypeByKey("key-SDF");
    protected Field F_name() => F_().FieldByKey("key-SDTfName");
    protected Field E_e2f() => E_().FieldByKey("key-SDTf");
    protected Field E_name() => E_().FieldByKey("key-SDTeName");

    protected Enumeration Currency_() => _sdtLang.Enumerations().FirstOrDefault(c => c.Key == "key-SDTCurrency")!;

    protected EnumerationLiteral Currency(string currency) =>
        Currency_().Literals.FirstOrDefault(l => l.Name == currency)!;

    protected Enum CurrencyValue(string currency) => _sdtLang.GetFactory().GetEnumerationLiteral(Currency(currency));
    protected Enum CurrencyEUR() => CurrencyValue("EUR");

    protected StructuredDataType Amount_() => _sdtLang.StructuredDataTypeByKey("key-SDTAmount");
    protected Field Amount_value() => Amount_().FieldByKey("key-SDTValue");
    protected Field Amount_digital() => Amount_().FieldByKey("key-SDTDigital");
    protected Field Amount_currency() => Amount_().FieldByKey("key-SDTCurr");

    protected IStructuredDataTypeInstance NewAmount() =>
        _sdtLang.GetFactory().CreateStructuredDataTypeInstance(Amount_(),
            new FieldValues { }
        );

    protected IStructuredDataTypeInstance NewAmount(Enum cur) =>
        _sdtLang.GetFactory().CreateStructuredDataTypeInstance(
            Amount_(),
            new FieldValues { { Amount_currency(), cur } }
        );


    protected IStructuredDataTypeInstance NewAmount(int dec, int frac, string currency, bool digital) =>
        _sdtLang.GetFactory().CreateStructuredDataTypeInstance(Amount_(),
            new FieldValues
            {
                { Amount_value(), NewDecimal(dec, frac) },
                { Amount_currency(), CurrencyValue(currency) },
                { Amount_digital(), digital }
            }
        );
}