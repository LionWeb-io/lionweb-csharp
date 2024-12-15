// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Examples.Mixed.MixedDirectSdtLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2024_1;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-mixedDirectSdtLang", Version = "1")]
public partial class MixedDirectSdtLangLanguage : LanguageBase<IMixedDirectSdtLangFactory>
{
	public static readonly MixedDirectSdtLangLanguage Instance = new Lazy<MixedDirectSdtLangLanguage>(() => new("id-mixedDirectSdtLang")).Value;
	public MixedDirectSdtLangLanguage(string id) : base(id, LionWebVersions.v2024_1)
	{
		_directSdt = new(() => new StructuredDataTypeBase<MixedDirectSdtLangLanguage>("id-directSdt", this) { Key = "key-directSdt", Name = "DirectSdt", FieldsLazy = new(() => [DirectSdt_directSdtEnum, DirectSdt_directSdtSdt]) });
		_directSdt_directSdtEnum = new(() => new FieldBase<MixedDirectSdtLangLanguage>("id-directSdtEnum", DirectSdt, this) { Key = "key-directSdtEnum", Name = "directSdtEnum", Type = Examples.Mixed.MixedNestedEnumLang.MixedNestedEnumLangLanguage.Instance.NestedEnum });
		_directSdt_directSdtSdt = new(() => new FieldBase<MixedDirectSdtLangLanguage>("id-directSdtSdt", DirectSdt, this) { Key = "key-directSdtSdt", Name = "directSdtSdt", Type = Examples.Mixed.MixedNestedSdtLang.MixedNestedSdtLangLanguage.Instance.NestedSdt });
		_factory = new MixedDirectSdtLangFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [DirectSdt];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [Examples.Mixed.MixedNestedEnumLang.MixedNestedEnumLangLanguage.Instance, Examples.Mixed.MixedNestedSdtLang.MixedNestedSdtLangLanguage.Instance];

	private const string _key = "key-mixedDirectSdtLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "MixedDirectSdtLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<StructuredDataType> _directSdt;
	public StructuredDataType DirectSdt => _directSdt.Value;

	private readonly Lazy<Field> _directSdt_directSdtEnum;
	public Field DirectSdt_directSdtEnum => _directSdt_directSdtEnum.Value;

	private readonly Lazy<Field> _directSdt_directSdtSdt;
	public Field DirectSdt_directSdtSdt => _directSdt_directSdtSdt.Value;
}

public partial interface IMixedDirectSdtLangFactory : INodeFactory
{
}

public class MixedDirectSdtLangFactory : AbstractBaseNodeFactory, IMixedDirectSdtLangFactory
{
	private readonly MixedDirectSdtLangLanguage _language;
	public MixedDirectSdtLangFactory(MixedDirectSdtLangLanguage language) : base(language)
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
		throw new UnsupportedEnumerationLiteralException(literal);
	}

	/// <inheritdoc/>
        public override IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType, IFieldValues fieldValues)
	{
		if (_language.DirectSdt.EqualsIdentity(structuredDataType))
			return new DirectSdt((Examples.Mixed.MixedNestedEnumLang.NestedEnum?)fieldValues.Get(_language.DirectSdt_directSdtEnum), (Examples.Mixed.MixedNestedSdtLang.NestedSdt?)fieldValues.Get(_language.DirectSdt_directSdtSdt));
		throw new UnsupportedStructuredDataTypeException(structuredDataType);
	}
}

[LionCoreMetaPointer(Language = typeof(MixedDirectSdtLangLanguage), Key = "key-directSdt")]
public readonly record struct DirectSdt : IStructuredDataTypeInstance
{
	private readonly Examples.Mixed.MixedNestedEnumLang.NestedEnum? _directSdtEnum;
	[LionCoreMetaPointer(Language = typeof(MixedDirectSdtLangLanguage), Key = "key-directSdtEnum")]
	public Examples.Mixed.MixedNestedEnumLang.NestedEnum DirectSdtEnum { get => _directSdtEnum ?? throw new UnsetFieldException(MixedDirectSdtLangLanguage.Instance.DirectSdt_directSdtEnum); init => _directSdtEnum = value; }

	private readonly Examples.Mixed.MixedNestedSdtLang.NestedSdt? _directSdtSdt;
	[LionCoreMetaPointer(Language = typeof(MixedDirectSdtLangLanguage), Key = "key-directSdtSdt")]
	public Examples.Mixed.MixedNestedSdtLang.NestedSdt DirectSdtSdt { get => _directSdtSdt ?? throw new UnsetFieldException(MixedDirectSdtLangLanguage.Instance.DirectSdt_directSdtSdt); init => _directSdtSdt = value; }

	public DirectSdt()
	{
		_directSdtEnum = null;
		_directSdtSdt = null;
	}

	internal DirectSdt(Examples.Mixed.MixedNestedEnumLang.NestedEnum? directSdtEnum_, Examples.Mixed.MixedNestedSdtLang.NestedSdt? directSdtSdt_)
	{
		_directSdtEnum = directSdtEnum_;
		_directSdtSdt = directSdtSdt_;
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => MixedDirectSdtLangLanguage.Instance.DirectSdt;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_directSdtEnum != null)
			result.Add(MixedDirectSdtLangLanguage.Instance.DirectSdt_directSdtEnum);
		if (_directSdtSdt != null)
			result.Add(MixedDirectSdtLangLanguage.Instance.DirectSdt_directSdtSdt);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (MixedDirectSdtLangLanguage.Instance.DirectSdt_directSdtEnum.EqualsIdentity(field))
			return DirectSdtEnum;
		if (MixedDirectSdtLangLanguage.Instance.DirectSdt_directSdtSdt.EqualsIdentity(field))
			return DirectSdtSdt;
		throw new UnsetFieldException(field);
	}
}