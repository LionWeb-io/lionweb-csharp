// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Examples.V2024_1.SDTLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2024_1;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-SDTLang", Version = "0")]
public partial class SDTLangLanguage : LanguageBase<ISDTLangFactory>
{
	public static readonly SDTLangLanguage Instance = new Lazy<SDTLangLanguage>(() => new("id-SDTLang")).Value;
	public SDTLangLanguage(string id) : base(id, LionWebVersions.v2024_1)
	{
		_a = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDTA", this) { Key = "key-SDTA", Name = "A", FieldsLazy = new(() => [A_name, A_a2b, A_a2c]) });
		_a_name = new(() => new FieldBase<SDTLangLanguage>("id-SDTaName", A, this) { Key = "key-SDTaName", Name = "name", Type = _builtIns.String });
		_a_a2b = new(() => new FieldBase<SDTLangLanguage>("id-SDTa2b", A, this) { Key = "key-SDTa2b", Name = "a2b", Type = B });
		_a_a2c = new(() => new FieldBase<SDTLangLanguage>("id-SDTa2c", A, this) { Key = "key-SDTa2c", Name = "a2c", Type = C });
		_amount = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDTAmount", this) { Key = "key-SDTAmount", Name = "Amount", FieldsLazy = new(() => [Amount_value, Amount_currency, Amount_digital]) });
		_amount_value = new(() => new FieldBase<SDTLangLanguage>("id-SDTValue", Amount, this) { Key = "key-SDTValue", Name = "value", Type = Decimal });
		_amount_currency = new(() => new FieldBase<SDTLangLanguage>("id-SDTCurr", Amount, this) { Key = "key-SDTCurr", Name = "currency", Type = Currency });
		_amount_digital = new(() => new FieldBase<SDTLangLanguage>("id-SDTDigital", Amount, this) { Key = "key-SDTDigital", Name = "digital", Type = _builtIns.Boolean });
		_b = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDTB", this) { Key = "key-SDTB", Name = "B", FieldsLazy = new(() => [B_name, B_b2d]) });
		_b_name = new(() => new FieldBase<SDTLangLanguage>("id-SDTbName", B, this) { Key = "key-SDTbName", Name = "name", Type = _builtIns.String });
		_b_b2d = new(() => new FieldBase<SDTLangLanguage>("id-SDTb2d", B, this) { Key = "key-SDTb2d", Name = "b2d", Type = D });
		_c = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDCT", this) { Key = "key-SDCT", Name = "C", FieldsLazy = new(() => [C_name, C_c2d, C_c2e]) });
		_c_name = new(() => new FieldBase<SDTLangLanguage>("id-SDTcName", C, this) { Key = "key-SDTcName", Name = "name", Type = _builtIns.String });
		_c_c2d = new(() => new FieldBase<SDTLangLanguage>("id-SDTc2d", C, this) { Key = "key-SDTc2d", Name = "c2d", Type = D });
		_c_c2e = new(() => new FieldBase<SDTLangLanguage>("id-SDTc2e", C, this) { Key = "key-SDTc2e", Name = "c2e", Type = E });
		_complexNumber = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDTComplexNumber", this) { Key = "key-SDTComplexNumber", Name = "ComplexNumber", FieldsLazy = new(() => [ComplexNumber_real, ComplexNumber_imaginary]) });
		_complexNumber_real = new(() => new FieldBase<SDTLangLanguage>("id-SDTReal", ComplexNumber, this) { Key = "key-SDTReal", Name = "real", Type = Decimal });
		_complexNumber_imaginary = new(() => new FieldBase<SDTLangLanguage>("id-SDTImaginary", ComplexNumber, this) { Key = "key-SDTImaginary", Name = "imaginary", Type = Decimal });
		_currency = new(() => new EnumerationBase<SDTLangLanguage>("id-SDTCurrency", this) { Key = "key-SDTCurrency", Name = "Currency", LiteralsLazy = new(() => [Currency_EUR, Currency_GBP]) });
		_currency_EUR = new(() => new EnumerationLiteralBase<SDTLangLanguage>("id-SDT-eur", Currency, this) { Key = "key-SDTEur", Name = "EUR" });
		_currency_GBP = new(() => new EnumerationLiteralBase<SDTLangLanguage>("id-SDT-gbp", Currency, this) { Key = "key-SDTGbp", Name = "GBP" });
		_d = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDD", this) { Key = "key-SDD", Name = "D", FieldsLazy = new(() => [D_name]) });
		_d_name = new(() => new FieldBase<SDTLangLanguage>("id-SDTdName", D, this) { Key = "key-SDTdName", Name = "name", Type = _builtIns.String });
		_decimal = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDTDecimal", this) { Key = "key-SDTDecimal", Name = "Decimal", FieldsLazy = new(() => [Decimal_int, Decimal_frac]) });
		_decimal_int = new(() => new FieldBase<SDTLangLanguage>("id-SDTInt", Decimal, this) { Key = "key-SDTInt", Name = "int", Type = _builtIns.Integer });
		_decimal_frac = new(() => new FieldBase<SDTLangLanguage>("id-SDTFrac", Decimal, this) { Key = "key-SDTFrac", Name = "frac", Type = _builtIns.Integer });
		_e = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDE", this) { Key = "key-SDE", Name = "E", FieldsLazy = new(() => [E_name, E_e2f, E_e2b]) });
		_e_name = new(() => new FieldBase<SDTLangLanguage>("id-SDTeName", E, this) { Key = "key-SDTeName", Name = "name", Type = _builtIns.String });
		_e_e2f = new(() => new FieldBase<SDTLangLanguage>("id-SDTe2f", E, this) { Key = "key-SDTf", Name = "e2f", Type = F });
		_e_e2b = new(() => new FieldBase<SDTLangLanguage>("id-SDTe2b", E, this) { Key = "key-SDTb", Name = "e2b", Type = B });
		_f = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDF", this) { Key = "key-SDF", Name = "F", FieldsLazy = new(() => [F_name, F_f2c]) });
		_f_name = new(() => new FieldBase<SDTLangLanguage>("id-SDTfName", F, this) { Key = "key-SDTfName", Name = "name", Type = _builtIns.String });
		_f_f2c = new(() => new FieldBase<SDTLangLanguage>("id-SDTf2c", F, this) { Key = "key-SDTf2c", Name = "f2c", Type = C });
		_fullyQualifiedName = new(() => new StructuredDataTypeBase<SDTLangLanguage>("id-SDTFullyQualifiedName", this) { Key = "key-SDTFullyQualifiedName", Name = "FullyQualifiedName", FieldsLazy = new(() => [FullyQualifiedName_name, FullyQualifiedName_nested]) });
		_fullyQualifiedName_name = new(() => new FieldBase<SDTLangLanguage>("id-SDT-FQN-name", FullyQualifiedName, this) { Key = "key-SDTFqnName", Name = "name", Type = _builtIns.String });
		_fullyQualifiedName_nested = new(() => new FieldBase<SDTLangLanguage>("id-SDT-FQN-nested", FullyQualifiedName, this) { Key = "key-SDTFqnNested", Name = "nested", Type = FullyQualifiedName });
		_sDTConcept = new(() => new ConceptBase<SDTLangLanguage>("id-SDTConcept", this) { Key = "key-SDTConcept", Name = "SDTConcept", Abstract = false, Partition = false, FeaturesLazy = new(() => [SDTConcept_A, SDTConcept_amount, SDTConcept_complex, SDTConcept_decimal, SDTConcept_fqn]) });
		_sDTConcept_A = new(() => new PropertyBase<SDTLangLanguage>("id-SDTAField", SDTConcept, this) { Key = "key-SDTAField", Name = "A", Optional = false, Type = A });
		_sDTConcept_amount = new(() => new PropertyBase<SDTLangLanguage>("id-SDTamountField", SDTConcept, this) { Key = "key-SDTamountField", Name = "amount", Optional = false, Type = Amount });
		_sDTConcept_complex = new(() => new PropertyBase<SDTLangLanguage>("id-SDTComplexField", SDTConcept, this) { Key = "key-SDTComplexField", Name = "complex", Optional = false, Type = ComplexNumber });
		_sDTConcept_decimal = new(() => new PropertyBase<SDTLangLanguage>("id-SDTDecimalField", SDTConcept, this) { Key = "key-SDTDecimalField", Name = "decimal", Optional = true, Type = Decimal });
		_sDTConcept_fqn = new(() => new PropertyBase<SDTLangLanguage>("id-SDTFqnField", SDTConcept, this) { Key = "key-SDTFqnField", Name = "fqn", Optional = false, Type = FullyQualifiedName });
		_factory = new SDTLangFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [A, Amount, B, C, ComplexNumber, Currency, D, Decimal, E, F, FullyQualifiedName, SDTConcept];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	private const string _key = "key-SDTLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "SDTLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "0";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<StructuredDataType> _a;
	public StructuredDataType A => _a.Value;

	private readonly Lazy<Field> _a_name;
	public Field A_name => _a_name.Value;

	private readonly Lazy<Field> _a_a2b;
	public Field A_a2b => _a_a2b.Value;

	private readonly Lazy<Field> _a_a2c;
	public Field A_a2c => _a_a2c.Value;

	private readonly Lazy<StructuredDataType> _amount;
	public StructuredDataType Amount => _amount.Value;

	private readonly Lazy<Field> _amount_value;
	public Field Amount_value => _amount_value.Value;

	private readonly Lazy<Field> _amount_currency;
	public Field Amount_currency => _amount_currency.Value;

	private readonly Lazy<Field> _amount_digital;
	public Field Amount_digital => _amount_digital.Value;

	private readonly Lazy<StructuredDataType> _b;
	public StructuredDataType B => _b.Value;

	private readonly Lazy<Field> _b_name;
	public Field B_name => _b_name.Value;

	private readonly Lazy<Field> _b_b2d;
	public Field B_b2d => _b_b2d.Value;

	private readonly Lazy<StructuredDataType> _c;
	public StructuredDataType C => _c.Value;

	private readonly Lazy<Field> _c_name;
	public Field C_name => _c_name.Value;

	private readonly Lazy<Field> _c_c2d;
	public Field C_c2d => _c_c2d.Value;

	private readonly Lazy<Field> _c_c2e;
	public Field C_c2e => _c_c2e.Value;

	private readonly Lazy<StructuredDataType> _complexNumber;
	public StructuredDataType ComplexNumber => _complexNumber.Value;

	private readonly Lazy<Field> _complexNumber_real;
	public Field ComplexNumber_real => _complexNumber_real.Value;

	private readonly Lazy<Field> _complexNumber_imaginary;
	public Field ComplexNumber_imaginary => _complexNumber_imaginary.Value;

	private readonly Lazy<Enumeration> _currency;
	public Enumeration Currency => _currency.Value;

	private readonly Lazy<EnumerationLiteral> _currency_EUR;
	public EnumerationLiteral Currency_EUR => _currency_EUR.Value;

	private readonly Lazy<EnumerationLiteral> _currency_GBP;
	public EnumerationLiteral Currency_GBP => _currency_GBP.Value;

	private readonly Lazy<StructuredDataType> _d;
	public StructuredDataType D => _d.Value;

	private readonly Lazy<Field> _d_name;
	public Field D_name => _d_name.Value;

	private readonly Lazy<StructuredDataType> _decimal;
	public StructuredDataType Decimal => _decimal.Value;

	private readonly Lazy<Field> _decimal_int;
	public Field Decimal_int => _decimal_int.Value;

	private readonly Lazy<Field> _decimal_frac;
	public Field Decimal_frac => _decimal_frac.Value;

	private readonly Lazy<StructuredDataType> _e;
	public StructuredDataType E => _e.Value;

	private readonly Lazy<Field> _e_name;
	public Field E_name => _e_name.Value;

	private readonly Lazy<Field> _e_e2f;
	public Field E_e2f => _e_e2f.Value;

	private readonly Lazy<Field> _e_e2b;
	public Field E_e2b => _e_e2b.Value;

	private readonly Lazy<StructuredDataType> _f;
	public StructuredDataType F => _f.Value;

	private readonly Lazy<Field> _f_name;
	public Field F_name => _f_name.Value;

	private readonly Lazy<Field> _f_f2c;
	public Field F_f2c => _f_f2c.Value;

	private readonly Lazy<StructuredDataType> _fullyQualifiedName;
	public StructuredDataType FullyQualifiedName => _fullyQualifiedName.Value;

	private readonly Lazy<Field> _fullyQualifiedName_name;
	public Field FullyQualifiedName_name => _fullyQualifiedName_name.Value;

	private readonly Lazy<Field> _fullyQualifiedName_nested;
	public Field FullyQualifiedName_nested => _fullyQualifiedName_nested.Value;

	private readonly Lazy<Concept> _sDTConcept;
	public Concept SDTConcept => _sDTConcept.Value;

	private readonly Lazy<Property> _sDTConcept_A;
	public Property SDTConcept_A => _sDTConcept_A.Value;

	private readonly Lazy<Property> _sDTConcept_amount;
	public Property SDTConcept_amount => _sDTConcept_amount.Value;

	private readonly Lazy<Property> _sDTConcept_complex;
	public Property SDTConcept_complex => _sDTConcept_complex.Value;

	private readonly Lazy<Property> _sDTConcept_decimal;
	public Property SDTConcept_decimal => _sDTConcept_decimal.Value;

	private readonly Lazy<Property> _sDTConcept_fqn;
	public Property SDTConcept_fqn => _sDTConcept_fqn.Value;
}

public partial interface ISDTLangFactory : INodeFactory
{
	public SDTConcept NewSDTConcept(string id);
	public SDTConcept CreateSDTConcept();
}

public class SDTLangFactory : AbstractBaseNodeFactory, ISDTLangFactory
{
	private readonly SDTLangLanguage _language;
	public SDTLangFactory(SDTLangLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.SDTConcept.EqualsIdentity(classifier))
			return NewSDTConcept(id);
		throw new UnsupportedClassifierException(classifier);
	}

	/// <inheritdoc/>
        public override Enum GetEnumerationLiteral(EnumerationLiteral literal)
	{
		if (_language.Currency.EqualsIdentity(literal.GetEnumeration()))
			return EnumValueFor<Currency>(literal);
		throw new UnsupportedEnumerationLiteralException(literal);
	}

	/// <inheritdoc/>
        public override IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType, IFieldValues fieldValues)
	{
		if (_language.A.EqualsIdentity(structuredDataType))
			return new A((string?)fieldValues.Get(_language.A_name), (B?)fieldValues.Get(_language.A_a2b), (C?)fieldValues.Get(_language.A_a2c));
		if (_language.Amount.EqualsIdentity(structuredDataType))
			return new Amount((Decimal?)fieldValues.Get(_language.Amount_value), (Currency?)fieldValues.Get(_language.Amount_currency), (bool?)fieldValues.Get(_language.Amount_digital));
		if (_language.B.EqualsIdentity(structuredDataType))
			return new B((string?)fieldValues.Get(_language.B_name), (D?)fieldValues.Get(_language.B_b2d));
		if (_language.C.EqualsIdentity(structuredDataType))
			return new C((string?)fieldValues.Get(_language.C_name), (D?)fieldValues.Get(_language.C_c2d), (E?)fieldValues.Get(_language.C_c2e));
		if (_language.ComplexNumber.EqualsIdentity(structuredDataType))
			return new ComplexNumber((Decimal?)fieldValues.Get(_language.ComplexNumber_real), (Decimal?)fieldValues.Get(_language.ComplexNumber_imaginary));
		if (_language.D.EqualsIdentity(structuredDataType))
			return new D((string?)fieldValues.Get(_language.D_name));
		if (_language.Decimal.EqualsIdentity(structuredDataType))
			return new Decimal((int?)fieldValues.Get(_language.Decimal_int), (int?)fieldValues.Get(_language.Decimal_frac));
		if (_language.E.EqualsIdentity(structuredDataType))
			return new E((string?)fieldValues.Get(_language.E_name), (F?)fieldValues.Get(_language.E_e2f), (B?)fieldValues.Get(_language.E_e2b));
		if (_language.F.EqualsIdentity(structuredDataType))
			return new F((string?)fieldValues.Get(_language.F_name), (C?)fieldValues.Get(_language.F_f2c));
		if (_language.FullyQualifiedName.EqualsIdentity(structuredDataType))
			return new FullyQualifiedName((string?)fieldValues.Get(_language.FullyQualifiedName_name), (FullyQualifiedName?)fieldValues.Get(_language.FullyQualifiedName_nested));
		throw new UnsupportedStructuredDataTypeException(structuredDataType);
	}

	public virtual SDTConcept NewSDTConcept(string id) => new(id);
	public virtual SDTConcept CreateSDTConcept() => NewSDTConcept(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTConcept")]
public partial class SDTConcept : NodeBase
{
	private A? _a = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If A has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTAField")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public A A { get => _a ?? throw new UnsetFeatureException(SDTLangLanguage.Instance.SDTConcept_A); set => SetA(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public SDTConcept SetA(A value)
	{
		AssureNotNull(value, SDTLangLanguage.Instance.SDTConcept_A);
		_a = value;
		return this;
	}

	private Amount? _amount = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Amount has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTamountField")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public Amount Amount { get => _amount ?? throw new UnsetFeatureException(SDTLangLanguage.Instance.SDTConcept_amount); set => SetAmount(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public SDTConcept SetAmount(Amount value)
	{
		AssureNotNull(value, SDTLangLanguage.Instance.SDTConcept_amount);
		_amount = value;
		return this;
	}

	private ComplexNumber? _complex = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Complex has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTComplexField")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public ComplexNumber Complex { get => _complex ?? throw new UnsetFeatureException(SDTLangLanguage.Instance.SDTConcept_complex); set => SetComplex(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public SDTConcept SetComplex(ComplexNumber value)
	{
		AssureNotNull(value, SDTLangLanguage.Instance.SDTConcept_complex);
		_complex = value;
		return this;
	}

	private Decimal? _decimal = null;
	/// <remarks>Optional Property</remarks>
        [LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTDecimalField")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = true, Multiple = false)]
	public Decimal? Decimal { get => _decimal; set => SetDecimal(value); }

	/// <remarks>Optional Property</remarks>
        public SDTConcept SetDecimal(Decimal? value)
	{
		_decimal = value;
		return this;
	}

	private FullyQualifiedName? _fqn = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Fqn has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTFqnField")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public FullyQualifiedName Fqn { get => _fqn ?? throw new UnsetFeatureException(SDTLangLanguage.Instance.SDTConcept_fqn); set => SetFqn(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public SDTConcept SetFqn(FullyQualifiedName value)
	{
		AssureNotNull(value, SDTLangLanguage.Instance.SDTConcept_fqn);
		_fqn = value;
		return this;
	}

	public SDTConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Classifier GetClassifier() => SDTLangLanguage.Instance.SDTConcept;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (SDTLangLanguage.Instance.SDTConcept_A.EqualsIdentity(feature))
		{
			result = A;
			return true;
		}

		if (SDTLangLanguage.Instance.SDTConcept_amount.EqualsIdentity(feature))
		{
			result = Amount;
			return true;
		}

		if (SDTLangLanguage.Instance.SDTConcept_complex.EqualsIdentity(feature))
		{
			result = Complex;
			return true;
		}

		if (SDTLangLanguage.Instance.SDTConcept_decimal.EqualsIdentity(feature))
		{
			result = Decimal;
			return true;
		}

		if (SDTLangLanguage.Instance.SDTConcept_fqn.EqualsIdentity(feature))
		{
			result = Fqn;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (SDTLangLanguage.Instance.SDTConcept_A.EqualsIdentity(feature))
		{
			if (value is Examples.V2024_1.SDTLang.A v)
			{
				A = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (SDTLangLanguage.Instance.SDTConcept_amount.EqualsIdentity(feature))
		{
			if (value is Examples.V2024_1.SDTLang.Amount v)
			{
				Amount = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (SDTLangLanguage.Instance.SDTConcept_complex.EqualsIdentity(feature))
		{
			if (value is Examples.V2024_1.SDTLang.ComplexNumber v)
			{
				Complex = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (SDTLangLanguage.Instance.SDTConcept_decimal.EqualsIdentity(feature))
		{
			if (value is null or Examples.V2024_1.SDTLang.Decimal)
			{
				Decimal = (Examples.V2024_1.SDTLang.Decimal?)value;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (SDTLangLanguage.Instance.SDTConcept_fqn.EqualsIdentity(feature))
		{
			if (value is Examples.V2024_1.SDTLang.FullyQualifiedName v)
			{
				Fqn = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		return false;
	}

	/// <inheritdoc/>
        public override IEnumerable<Feature> CollectAllSetFeatures()
	{
		var result = base.CollectAllSetFeatures().ToList();
		if (_a != default)
			result.Add(SDTLangLanguage.Instance.SDTConcept_A);
		if (_amount != default)
			result.Add(SDTLangLanguage.Instance.SDTConcept_amount);
		if (_complex != default)
			result.Add(SDTLangLanguage.Instance.SDTConcept_complex);
		if (_decimal != default)
			result.Add(SDTLangLanguage.Instance.SDTConcept_decimal);
		if (_fqn != default)
			result.Add(SDTLangLanguage.Instance.SDTConcept_fqn);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTCurrency")]
public enum Currency
{
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTEur")]
	@EUR,
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTGbp")]
	@GBP
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTA")]
public readonly record struct A : IStructuredDataTypeInstance
{
	private readonly B? _a2b;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTa2b")]
	public B A2b { get => _a2b ?? throw new UnsetFieldException(SDTLangLanguage.Instance.A_a2b); init => _a2b = value; }

	private readonly NullableStructMember<C> _a2c;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTa2c")]
	public C? A2c { get => _a2c.Value; init => _a2c = new(value); }

	private readonly string? _name;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTaName")]
	public string Name { get => _name ?? throw new UnsetFieldException(SDTLangLanguage.Instance.A_name); init => _name = value; }

	public A()
	{
		_name = null;
		_a2b = null;
		_a2c = new(null);
	}

	internal A(string? name_, B? a2b_, C? a2c_)
	{
		_name = name_;
		_a2b = a2b_;
		_a2c = new(a2c_);
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.A;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_name != null)
			result.Add(SDTLangLanguage.Instance.A_name);
		if (_a2b != null)
			result.Add(SDTLangLanguage.Instance.A_a2b);
		if (_a2c != null && _a2c.Value != null)
			result.Add(SDTLangLanguage.Instance.A_a2c);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.A_name.EqualsIdentity(field))
			return Name;
		if (SDTLangLanguage.Instance.A_a2b.EqualsIdentity(field))
			return A2b;
		if (SDTLangLanguage.Instance.A_a2c.EqualsIdentity(field) && _a2c.Value != null)
			return A2c;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTAmount")]
public readonly record struct Amount : IStructuredDataTypeInstance
{
	private readonly Currency? _currency;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTCurr")]
	public Currency Currency { get => _currency ?? throw new UnsetFieldException(SDTLangLanguage.Instance.Amount_currency); init => _currency = value; }

	private readonly bool? _digital;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTDigital")]
	public bool Digital { get => _digital ?? throw new UnsetFieldException(SDTLangLanguage.Instance.Amount_digital); init => _digital = value; }

	private readonly Decimal? _value;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTValue")]
	public Decimal Value { get => _value ?? throw new UnsetFieldException(SDTLangLanguage.Instance.Amount_value); init => _value = value; }

	public Amount()
	{
		_value = null;
		_currency = null;
		_digital = null;
	}

	internal Amount(Decimal? value_, Currency? currency_, bool? digital_)
	{
		_value = value_;
		_currency = currency_;
		_digital = digital_;
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.Amount;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_value != null)
			result.Add(SDTLangLanguage.Instance.Amount_value);
		if (_currency != null)
			result.Add(SDTLangLanguage.Instance.Amount_currency);
		if (_digital != null)
			result.Add(SDTLangLanguage.Instance.Amount_digital);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.Amount_value.EqualsIdentity(field))
			return Value;
		if (SDTLangLanguage.Instance.Amount_currency.EqualsIdentity(field))
			return Currency;
		if (SDTLangLanguage.Instance.Amount_digital.EqualsIdentity(field))
			return Digital;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTB")]
public readonly record struct B : IStructuredDataTypeInstance
{
	private readonly D? _b2d;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTb2d")]
	public D B2d { get => _b2d ?? throw new UnsetFieldException(SDTLangLanguage.Instance.B_b2d); init => _b2d = value; }

	private readonly string? _name;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTbName")]
	public string Name { get => _name ?? throw new UnsetFieldException(SDTLangLanguage.Instance.B_name); init => _name = value; }

	public B()
	{
		_name = null;
		_b2d = null;
	}

	internal B(string? name_, D? b2d_)
	{
		_name = name_;
		_b2d = b2d_;
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.B;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_name != null)
			result.Add(SDTLangLanguage.Instance.B_name);
		if (_b2d != null)
			result.Add(SDTLangLanguage.Instance.B_b2d);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.B_name.EqualsIdentity(field))
			return Name;
		if (SDTLangLanguage.Instance.B_b2d.EqualsIdentity(field))
			return B2d;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDCT")]
public readonly record struct C : IStructuredDataTypeInstance
{
	private readonly D? _c2d;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTc2d")]
	public D C2d { get => _c2d ?? throw new UnsetFieldException(SDTLangLanguage.Instance.C_c2d); init => _c2d = value; }

	private readonly NullableStructMember<E> _c2e;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTc2e")]
	public E? C2e { get => _c2e.Value; init => _c2e = new(value); }

	private readonly string? _name;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTcName")]
	public string Name { get => _name ?? throw new UnsetFieldException(SDTLangLanguage.Instance.C_name); init => _name = value; }

	public C()
	{
		_name = null;
		_c2d = null;
		_c2e = new(null);
	}

	internal C(string? name_, D? c2d_, E? c2e_)
	{
		_name = name_;
		_c2d = c2d_;
		_c2e = new(c2e_);
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.C;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_name != null)
			result.Add(SDTLangLanguage.Instance.C_name);
		if (_c2d != null)
			result.Add(SDTLangLanguage.Instance.C_c2d);
		if (_c2e != null && _c2e.Value != null)
			result.Add(SDTLangLanguage.Instance.C_c2e);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.C_name.EqualsIdentity(field))
			return Name;
		if (SDTLangLanguage.Instance.C_c2d.EqualsIdentity(field))
			return C2d;
		if (SDTLangLanguage.Instance.C_c2e.EqualsIdentity(field) && _c2e.Value != null)
			return C2e;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTComplexNumber")]
public readonly record struct ComplexNumber : IStructuredDataTypeInstance
{
	private readonly Decimal? _imaginary;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTImaginary")]
	public Decimal Imaginary { get => _imaginary ?? throw new UnsetFieldException(SDTLangLanguage.Instance.ComplexNumber_imaginary); init => _imaginary = value; }

	private readonly Decimal? _real;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTReal")]
	public Decimal Real { get => _real ?? throw new UnsetFieldException(SDTLangLanguage.Instance.ComplexNumber_real); init => _real = value; }

	public ComplexNumber()
	{
		_real = null;
		_imaginary = null;
	}

	internal ComplexNumber(Decimal? real_, Decimal? imaginary_)
	{
		_real = real_;
		_imaginary = imaginary_;
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.ComplexNumber;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_real != null)
			result.Add(SDTLangLanguage.Instance.ComplexNumber_real);
		if (_imaginary != null)
			result.Add(SDTLangLanguage.Instance.ComplexNumber_imaginary);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.ComplexNumber_real.EqualsIdentity(field))
			return Real;
		if (SDTLangLanguage.Instance.ComplexNumber_imaginary.EqualsIdentity(field))
			return Imaginary;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDD")]
public readonly record struct D : IStructuredDataTypeInstance
{
	private readonly string? _name;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTdName")]
	public string Name { get => _name ?? throw new UnsetFieldException(SDTLangLanguage.Instance.D_name); init => _name = value; }

	public D()
	{
		_name = null;
	}

	internal D(string? name_)
	{
		_name = name_;
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.D;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_name != null)
			result.Add(SDTLangLanguage.Instance.D_name);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.D_name.EqualsIdentity(field))
			return Name;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTDecimal")]
public readonly record struct Decimal : IStructuredDataTypeInstance
{
	private readonly int? _frac;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTFrac")]
	public int Frac { get => _frac ?? throw new UnsetFieldException(SDTLangLanguage.Instance.Decimal_frac); init => _frac = value; }

	private readonly int? _int;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTInt")]
	public int Int { get => _int ?? throw new UnsetFieldException(SDTLangLanguage.Instance.Decimal_int); init => _int = value; }

	public Decimal()
	{
		_int = null;
		_frac = null;
	}

	internal Decimal(int? int_, int? frac_)
	{
		_int = int_;
		_frac = frac_;
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.Decimal;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_int != null)
			result.Add(SDTLangLanguage.Instance.Decimal_int);
		if (_frac != null)
			result.Add(SDTLangLanguage.Instance.Decimal_frac);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.Decimal_int.EqualsIdentity(field))
			return Int;
		if (SDTLangLanguage.Instance.Decimal_frac.EqualsIdentity(field))
			return Frac;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDE")]
public readonly record struct E : IStructuredDataTypeInstance
{
	private readonly B? _e2b;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTb")]
	public B E2b { get => _e2b ?? throw new UnsetFieldException(SDTLangLanguage.Instance.E_e2b); init => _e2b = value; }

	private readonly NullableStructMember<F> _e2f;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTf")]
	public F? E2f { get => _e2f.Value; init => _e2f = new(value); }

	private readonly string? _name;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTeName")]
	public string Name { get => _name ?? throw new UnsetFieldException(SDTLangLanguage.Instance.E_name); init => _name = value; }

	public E()
	{
		_name = null;
		_e2f = new(null);
		_e2b = null;
	}

	internal E(string? name_, F? e2f_, B? e2b_)
	{
		_name = name_;
		_e2f = new(e2f_);
		_e2b = e2b_;
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.E;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_name != null)
			result.Add(SDTLangLanguage.Instance.E_name);
		if (_e2f != null && _e2f.Value != null)
			result.Add(SDTLangLanguage.Instance.E_e2f);
		if (_e2b != null)
			result.Add(SDTLangLanguage.Instance.E_e2b);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.E_name.EqualsIdentity(field))
			return Name;
		if (SDTLangLanguage.Instance.E_e2f.EqualsIdentity(field) && _e2f.Value != null)
			return E2f;
		if (SDTLangLanguage.Instance.E_e2b.EqualsIdentity(field))
			return E2b;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDF")]
public readonly record struct F : IStructuredDataTypeInstance
{
	private readonly NullableStructMember<C> _f2c;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTf2c")]
	public C? F2c { get => _f2c.Value; init => _f2c = new(value); }

	private readonly string? _name;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTfName")]
	public string Name { get => _name ?? throw new UnsetFieldException(SDTLangLanguage.Instance.F_name); init => _name = value; }

	public F()
	{
		_name = null;
		_f2c = new(null);
	}

	internal F(string? name_, C? f2c_)
	{
		_name = name_;
		_f2c = new(f2c_);
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.F;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_name != null)
			result.Add(SDTLangLanguage.Instance.F_name);
		if (_f2c != null && _f2c.Value != null)
			result.Add(SDTLangLanguage.Instance.F_f2c);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.F_name.EqualsIdentity(field))
			return Name;
		if (SDTLangLanguage.Instance.F_f2c.EqualsIdentity(field) && _f2c.Value != null)
			return F2c;
		throw new UnsetFieldException(field);
	}
}

[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTFullyQualifiedName")]
public readonly record struct FullyQualifiedName : IStructuredDataTypeInstance
{
	private readonly string? _name;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTFqnName")]
	public string Name { get => _name ?? throw new UnsetFieldException(SDTLangLanguage.Instance.FullyQualifiedName_name); init => _name = value; }

	private readonly NullableStructMember<FullyQualifiedName> _nested;
	[LionCoreMetaPointer(Language = typeof(SDTLangLanguage), Key = "key-SDTFqnNested")]
	public FullyQualifiedName? Nested { get => _nested.Value; init => _nested = new(value); }

	public FullyQualifiedName()
	{
		_name = null;
		_nested = new(null);
	}

	internal FullyQualifiedName(string? name_, FullyQualifiedName? nested_)
	{
		_name = name_;
		_nested = new(nested_);
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => SDTLangLanguage.Instance.FullyQualifiedName;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_name != null)
			result.Add(SDTLangLanguage.Instance.FullyQualifiedName_name);
		if (_nested != null && _nested.Value != null)
			result.Add(SDTLangLanguage.Instance.FullyQualifiedName_nested);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (SDTLangLanguage.Instance.FullyQualifiedName_name.EqualsIdentity(field))
			return Name;
		if (SDTLangLanguage.Instance.FullyQualifiedName_nested.EqualsIdentity(field) && _nested.Value != null)
			return Nested;
		throw new UnsetFieldException(field);
	}
}