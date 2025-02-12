// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace LionWeb.Core.Test.Languages.Generated.V2023_1.WithEnum.M2;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2023_1;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[LionCoreLanguage(Key = "WithEnum", Version = "1")]
public partial class WithEnumLanguage : LanguageBase<IWithEnumFactory>
{
	public static readonly WithEnumLanguage Instance = new Lazy<WithEnumLanguage>(() => new("RkXMUe9zMRUQpw3bzQjSbBX6ju1a0UplEYsVa799WPQ")).Value;
	public WithEnumLanguage(string id) : base(id, LionWebVersions.v2023_1)
	{
		_enumHolder = new(() => new ConceptBase<WithEnumLanguage>("PHrmcBTHnKIQD5HKKaWjhGp2U8Ndc99uZ0JftEqCjms", this) { Key = "EnumHolder", Name = "EnumHolder", Abstract = false, Partition = false, FeaturesLazy = new(() => [EnumHolder_enumValue]) });
		_enumHolder_enumValue = new(() => new PropertyBase<WithEnumLanguage>("kzHwP_f-H6_UOSDM4Au_XDc-Jb-sIcxw3vu7XvqotgU", EnumHolder, this) { Key = "enumValue", Name = "enumValue", Optional = false, Type = MyEnum });
		_myEnum = new(() => new EnumerationBase<WithEnumLanguage>("uLnvTaBlWPqfYfLjfN9HG9Qjt4VQHhFVRorDLFeYhZE", this) { Key = "MyEnum", Name = "MyEnum", LiteralsLazy = new(() => [MyEnum_literal1, MyEnum_literal2]) });
		_myEnum_literal1 = new(() => new EnumerationLiteralBase<WithEnumLanguage>("0euWZGmWfx0iPC66yajzWqv3gIv--pvo55hqY82nREc", MyEnum, this) { Key = "lit1", Name = "literal1" });
		_myEnum_literal2 = new(() => new EnumerationLiteralBase<WithEnumLanguage>("ArZRq3V1eIqmZBN88UlVbeRFfp3YVf-_JMd9-s64Yjg", MyEnum, this) { Key = "lit2", Name = "literal2" });
		_factory = new WithEnumFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [EnumHolder, MyEnum];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	private const string _key = "WithEnum";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "WithEnum";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _enumHolder;
	public Concept EnumHolder => _enumHolder.Value;

	private readonly Lazy<Property> _enumHolder_enumValue;
	public Property EnumHolder_enumValue => _enumHolder_enumValue.Value;

	private readonly Lazy<Enumeration> _myEnum;
	public Enumeration MyEnum => _myEnum.Value;

	private readonly Lazy<EnumerationLiteral> _myEnum_literal1;
	public EnumerationLiteral MyEnum_literal1 => _myEnum_literal1.Value;

	private readonly Lazy<EnumerationLiteral> _myEnum_literal2;
	public EnumerationLiteral MyEnum_literal2 => _myEnum_literal2.Value;
}

public partial interface IWithEnumFactory : INodeFactory
{
	public EnumHolder NewEnumHolder(string id);
	public EnumHolder CreateEnumHolder();
}

public class WithEnumFactory : AbstractBaseNodeFactory, IWithEnumFactory
{
	private readonly WithEnumLanguage _language;
	public WithEnumFactory(WithEnumLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.EnumHolder.EqualsIdentity(classifier))
			return NewEnumHolder(id);
		throw new UnsupportedClassifierException(classifier);
	}

	/// <inheritdoc/>
        public override Enum GetEnumerationLiteral(EnumerationLiteral literal)
	{
		if (_language.MyEnum.EqualsIdentity(literal.GetEnumeration()))
			return EnumValueFor<MyEnum>(literal);
		throw new UnsupportedEnumerationLiteralException(literal);
	}

	/// <inheritdoc/>
        public override IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType, IFieldValues fieldValues)
	{
		throw new UnsupportedStructuredDataTypeException(structuredDataType);
	}

	public virtual EnumHolder NewEnumHolder(string id) => new(id);
	public virtual EnumHolder CreateEnumHolder() => NewEnumHolder(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(WithEnumLanguage), Key = "EnumHolder")]
public partial class EnumHolder : ConceptInstanceBase
{
	private MyEnum? _enumValue = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If EnumValue has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(WithEnumLanguage), Key = "enumValue")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public MyEnum EnumValue { get => _enumValue ?? throw new UnsetFeatureException(WithEnumLanguage.Instance.EnumHolder_enumValue); set => SetEnumValue(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public bool TryGetEnumValue([MaybeNullWhenAttribute(false)] out MyEnum? enumValue)
	{
		enumValue = _enumValue;
		return _enumValue != null;
	}

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public EnumHolder SetEnumValue(MyEnum value)
	{
		AssureNotNull(value, WithEnumLanguage.Instance.EnumHolder_enumValue);
		_enumValue = value;
		return this;
	}

	public EnumHolder(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => WithEnumLanguage.Instance.EnumHolder;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (WithEnumLanguage.Instance.EnumHolder_enumValue.EqualsIdentity(feature))
		{
			result = EnumValue;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (WithEnumLanguage.Instance.EnumHolder_enumValue.EqualsIdentity(feature))
		{
			if (value is LionWeb.Core.Test.Languages.Generated.V2023_1.WithEnum.M2.MyEnum v)
			{
				EnumValue = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		return false;
	}

	/// <inheritdoc/>
        public override IEnumerable<Feature> CollectAllSetFeatures()
	{
		List<Feature> result = base.CollectAllSetFeatures().ToList();
		if (TryGetEnumValue(out _))
			result.Add(WithEnumLanguage.Instance.EnumHolder_enumValue);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(WithEnumLanguage), Key = "MyEnum")]
public enum MyEnum
{
	[LionCoreMetaPointer(Language = typeof(WithEnumLanguage), Key = "lit1")]
	literal1,
	[LionCoreMetaPointer(Language = typeof(WithEnumLanguage), Key = "lit2")]
	literal2
}