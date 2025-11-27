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

namespace LionWeb.Core.Test.NodeApi.Generated.StructuredDataType;

using Languages.Generated.V2024_1.SDTLang;
using M3;

public abstract class StructuredDataTypeTestBase
{
    protected SDTConcept NewSdtConcept(string id) => new SDTConcept(id);

    protected Property SdtConcept_decimal => SDTLangLanguage.Instance.SDTConcept_decimal;
    protected Field DecimalInt() => SDTLangLanguage.Instance.Decimal_int;
    protected Field DecimalFrac() => SDTLangLanguage.Instance.Decimal_frac;

    protected Decimal NewDecimal(int dec, int frac) => new Decimal(frac, dec);
    protected Decimal NewDecimal() => new();
    protected E NewE(F e2F) => new() { E2f = e2F };
    protected E NewE(string name) => new() { Name = name };
    protected E NewE(F e2F, string? name) => new() { E2f = e2F, Name = name! };
    protected F NewF(string name) => new(name);
    protected Field E_e2f() => SDTLangLanguage.Instance.E_e2f;
    protected Field E_name() => SDTLangLanguage.Instance.E_name;
    protected Currency CurrencyEUR() => Currency.EUR;

    protected Field Amount_value() => SDTLangLanguage.Instance.Amount_value;
    protected Field Amount_digital() => SDTLangLanguage.Instance.Amount_digital;
    protected Field Amount_currency() => SDTLangLanguage.Instance.Amount_currency;

    protected Amount NewAmount() => new();
    protected Amount NewAmount(Currency cur) => new() { Currency = cur };
    protected Amount NewAmount(int dec, int frac, string currency, bool digital)
        => new Amount(
            Enum.TryParse<Currency>(currency, out var result) ? result : null,
            digital,
            NewDecimal(dec, frac)
        );
}