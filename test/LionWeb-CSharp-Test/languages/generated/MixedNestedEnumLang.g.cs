// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Examples.Mixed.MixedNestedEnumLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-mixedNestedEnumLang", Version = "1")]
public class MixedNestedEnumLangLanguage : LanguageBase<IMixedNestedEnumLangFactory>
{
	public static readonly MixedNestedEnumLangLanguage Instance = new Lazy<MixedNestedEnumLangLanguage>(() => new("id-mixedNestedEnumLang")).Value;
	public MixedNestedEnumLangLanguage(string id) : base(id)
	{
		_nestedEnum = new(() => new EnumerationBase<MixedNestedEnumLangLanguage>("id-nestedEnum", this) { Key = "key-nestedEnum", Name = "NestedEnum", LiteralsLazy = new(() => [NestedEnum_nestedLiteralA]) });
		_nestedEnum_nestedLiteralA = new(() => new EnumerationLiteralBase<MixedNestedEnumLangLanguage>("id-nestedLiteralA", NestedEnum, this) { Key = "key-nestedLiteralA", Name = "nestedLiteralA" });
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [NestedEnum];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	/// <inheritdoc/>
        public override IMixedNestedEnumLangFactory GetFactory() => new MixedNestedEnumLangFactory(this);
	private const string _key = "key-mixedNestedEnumLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "MixedNestedEnumLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Enumeration> _nestedEnum;
	public Enumeration NestedEnum => _nestedEnum.Value;

	private readonly Lazy<EnumerationLiteral> _nestedEnum_nestedLiteralA;
	public EnumerationLiteral NestedEnum_nestedLiteralA => _nestedEnum_nestedLiteralA.Value;
}

public interface IMixedNestedEnumLangFactory : INodeFactory
{
}

public class MixedNestedEnumLangFactory : AbstractBaseNodeFactory, IMixedNestedEnumLangFactory
{
	private readonly MixedNestedEnumLangLanguage _language;
	public MixedNestedEnumLangFactory(MixedNestedEnumLangLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		throw new UnsupportedClassifierException(classifier);
	}

	/// <inheritdoc/>
        public override Enum GetEnumerationLiteral(EnumerationLiteral literal)
	{
		if (_language.NestedEnum.EqualsIdentity(literal.GetEnumeration()))
			return EnumValueFor<NestedEnum>(literal);
		throw new UnsupportedEnumerationLiteralException(literal);
	}

	/// <inheritdoc/>
        public override IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType, IFieldValues fieldValues)
	{
		throw new UnsupportedStructuredDataTypeException(structuredDataType);
	}
}

[LionCoreMetaPointer(Language = typeof(MixedNestedEnumLangLanguage), Key = "key-nestedEnum")]
public enum NestedEnum
{
	[LionCoreMetaPointer(Language = typeof(MixedNestedEnumLangLanguage), Key = "key-nestedLiteralA")]
	@nestedLiteralA
}