// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace LionWeb.Core.Test.Languages.Generated.V2025_1.Mixed.MixedConceptLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2025_1;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-mixedConceptLang", Version = "1")]
public partial class MixedConceptLangLanguage : LanguageBase<IMixedConceptLangFactory>
{
	public static readonly MixedConceptLangLanguage Instance = new Lazy<MixedConceptLangLanguage>(() => new("id-mixedConceptLang")).Value;
	public MixedConceptLangLanguage(string id) : base(id, LionWebVersions.v2025_1)
	{
		_mixedConcept = new(() => new ConceptBase<MixedConceptLangLanguage>("id-mixedConcept", this) { Key = "key-mixedConcept", Name = "MixedConcept", Abstract = false, Partition = false, ExtendsLazy = new(() => LionWeb.Core.Test.Languages.Generated.V2025_1.Mixed.MixedBaseConceptLang.MixedBaseConceptLangLanguage.Instance.BaseConcept) });
		_factory = new MixedConceptLangFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [MixedConcept];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [LionWeb.Core.Test.Languages.Generated.V2025_1.Mixed.MixedBaseConceptLang.MixedBaseConceptLangLanguage.Instance];

	private const string _key = "key-mixedConceptLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "MixedConceptLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _mixedConcept;
	public Concept MixedConcept => _mixedConcept.Value;
}

public partial interface IMixedConceptLangFactory : INodeFactory
{
	public MixedConcept NewMixedConcept(string id);
	public MixedConcept CreateMixedConcept();
}

public class MixedConceptLangFactory : AbstractBaseNodeFactory, IMixedConceptLangFactory
{
	private readonly MixedConceptLangLanguage _language;
	public MixedConceptLangFactory(MixedConceptLangLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.MixedConcept.EqualsIdentity(classifier))
			return NewMixedConcept(id);
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

	public virtual MixedConcept NewMixedConcept(string id) => new(id);
	public virtual MixedConcept CreateMixedConcept() => NewMixedConcept(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(MixedConceptLangLanguage), Key = "key-mixedConcept")]
public partial class MixedConcept : LionWeb.Core.Test.Languages.Generated.V2025_1.Mixed.MixedBaseConceptLang.BaseConcept
{
	public MixedConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => MixedConceptLangLanguage.Instance.MixedConcept;
}