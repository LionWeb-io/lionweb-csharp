// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Examples.Mixed.MixedBaseReferenceLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-mixedBaseReferenceLang", Version = "1")]
public class MixedBaseReferenceLangLanguage : LanguageBase<IMixedBaseReferenceLangFactory>
{
	public static readonly MixedBaseReferenceLangLanguage Instance = new Lazy<MixedBaseReferenceLangLanguage>(() => new("id-mixedBaseReferenceLang")).Value;
	public MixedBaseReferenceLangLanguage(string id) : base(id)
	{
		_baseReferenceIface = new(() => new InterfaceBase<MixedBaseReferenceLangLanguage>("id-baseReferenceIface", this) { Key = "key-baseReferenceIface", Name = "BaseReferenceIface", FeaturesLazy = new(() => [BaseReferenceIface_Ref]) });
		_baseReferenceIface_Ref = new(() => new ReferenceBase<MixedBaseReferenceLangLanguage>("id-baseReferenceIface-ref", BaseReferenceIface, this) { Key = "key-baseReferenceIface-ref", Name = "Ref", Optional = false, Multiple = false, Type = BuiltInsLanguage.Instance.Node });
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [BaseReferenceIface];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	/// <inheritdoc/>
        public override IMixedBaseReferenceLangFactory GetFactory() => new MixedBaseReferenceLangFactory(this);
	private const string _key = "key-mixedBaseReferenceLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "MixedBaseReferenceLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Interface> _baseReferenceIface;
	public Interface BaseReferenceIface => _baseReferenceIface.Value;

	private readonly Lazy<Reference> _baseReferenceIface_Ref;
	public Reference BaseReferenceIface_Ref => _baseReferenceIface_Ref.Value;
}

public interface IMixedBaseReferenceLangFactory : INodeFactory
{
}

public class MixedBaseReferenceLangFactory : AbstractBaseNodeFactory, IMixedBaseReferenceLangFactory
{
	private readonly MixedBaseReferenceLangLanguage _language;
	public MixedBaseReferenceLangFactory(MixedBaseReferenceLangLanguage language) : base(language)
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

[LionCoreMetaPointer(Language = typeof(MixedBaseReferenceLangLanguage), Key = "key-baseReferenceIface")]
public partial interface BaseReferenceIface : INode
{
	/// <remarks>Required Single Reference</remarks>
        [LionCoreMetaPointer(Language = typeof(MixedBaseReferenceLangLanguage), Key = "key-baseReferenceIface-ref")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Reference, Optional = false, Multiple = false)]
	public NodeBase Ref { get; set; }

	/// <remarks>Required Single Reference</remarks>
        public BaseReferenceIface SetRef(NodeBase value);
}