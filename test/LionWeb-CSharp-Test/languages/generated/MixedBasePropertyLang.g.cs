// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Examples.Mixed.MixedBasePropertyLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2024_1;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-mixedBasePropertyLang", Version = "1")]
public partial class MixedBasePropertyLangLanguage : LanguageBase<IMixedBasePropertyLangFactory>
{
	public static readonly MixedBasePropertyLangLanguage Instance = new Lazy<MixedBasePropertyLangLanguage>(() => new("id-mixedBasePropertyLang")).Value;
	public MixedBasePropertyLangLanguage(string id) : base(id, LionWebVersions.v2024_1)
	{
		_basePropertyIface = new(() => new InterfaceBase<MixedBasePropertyLangLanguage>("id-basePropertyIface", this) { Key = "key-basePropertyIface", Name = "BasePropertyIface", FeaturesLazy = new(() => [BasePropertyIface_Prop]) });
		_basePropertyIface_Prop = new(() => new PropertyBase<MixedBasePropertyLangLanguage>("id-basePropertyIface-prop", BasePropertyIface, this) { Key = "key-basePropertyIface-prop", Name = "Prop", Optional = false, Type = _builtIns.String });
		_factory = new MixedBasePropertyLangFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [BasePropertyIface];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	private const string _key = "key-mixedBasePropertyLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "MixedBasePropertyLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Interface> _basePropertyIface;
	public Interface BasePropertyIface => _basePropertyIface.Value;

	private readonly Lazy<Property> _basePropertyIface_Prop;
	public Property BasePropertyIface_Prop => _basePropertyIface_Prop.Value;
}

public partial interface IMixedBasePropertyLangFactory : INodeFactory
{
}

public class MixedBasePropertyLangFactory : AbstractBaseNodeFactory, IMixedBasePropertyLangFactory
{
	private readonly MixedBasePropertyLangLanguage _language;
	public MixedBasePropertyLangFactory(MixedBasePropertyLangLanguage language) : base(language)
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
		throw new UnsupportedStructuredDataTypeException(structuredDataType);
	}
}

[LionCoreMetaPointer(Language = typeof(MixedBasePropertyLangLanguage), Key = "key-basePropertyIface")]
public partial interface BasePropertyIface : INode
{
	/// <remarks>Required Property</remarks>
        [LionCoreMetaPointer(Language = typeof(MixedBasePropertyLangLanguage), Key = "key-basePropertyIface-prop")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public string Prop { get; set; }

	/// <remarks>Required Property</remarks>
        public BasePropertyIface SetProp(string value);
}