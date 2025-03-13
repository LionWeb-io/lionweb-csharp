// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.B;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2024_1;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// bLang desc
/// <seealso cref = "LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.ALangLanguage"/>
[LionCoreLanguage(Key = "key-BLang", Version = "2")]
public partial class BLangLanguage : LanguageBase<IBLangFactory>
{
	public static readonly BLangLanguage Instance = new Lazy<BLangLanguage>(() => new("id-BLang")).Value;
	public BLangLanguage(string id) : base(id, LionWebVersions.v2024_1)
	{
		_bConcept = new(() => new ConceptBase<BLangLanguage>("id-BConcept", this) { Key = "key-BConcept", Name = "BConcept", Abstract = false, Partition = false, FeaturesLazy = new(() => [BConcept_AEnumProp, BConcept_ARef]) });
		_bConcept_AEnumProp = new(() => new PropertyBase<BLangLanguage>("id-BConcept-AEnumProp", BConcept, this) { Key = "key-AEnumProp", Name = "AEnumProp", Optional = false, Type = LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.ALangLanguage.Instance.AEnum });
		_bConcept_ARef = new(() => new ReferenceBase<BLangLanguage>("id-BConcept-ARef", BConcept, this) { Key = "key-ARef", Name = "ARef", Optional = true, Multiple = false, Type = LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.ALangLanguage.Instance.AConcept });
		_factory = new BLangFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [BConcept];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.ALangLanguage.Instance];

	private const string _key = "key-BLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "BLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "2";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _bConcept;
	public Concept BConcept => _bConcept.Value;

	private readonly Lazy<Property> _bConcept_AEnumProp;
	public Property BConcept_AEnumProp => _bConcept_AEnumProp.Value;

	private readonly Lazy<Reference> _bConcept_ARef;
	public Reference BConcept_ARef => _bConcept_ARef.Value;
}

public partial interface IBLangFactory : INodeFactory
{
	public BConcept NewBConcept(string id);
	public BConcept CreateBConcept();
}

public class BLangFactory : AbstractBaseNodeFactory, IBLangFactory
{
	private readonly BLangLanguage _language;
	public BLangFactory(BLangLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.BConcept.EqualsIdentity(classifier))
			return NewBConcept(id);
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

	public virtual BConcept NewBConcept(string id) => new(id);
	public virtual BConcept CreateBConcept() => NewBConcept(GetNewId());
}

/// <summary>Some enum</summary>
[LionCoreMetaPointer(Language = typeof(BLangLanguage), Key = "key-BConcept")]
public partial class BConcept : ConceptInstanceBase
{
	private LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AEnum? _aEnumProp = null;
	/// enum desc
    	/// <seealso cref = "LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.ALangLanguage"/>
    	/// <seealso cref = "LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept.BRef"/>
    	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If AEnumProp has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(BLangLanguage), Key = "key-AEnumProp")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AEnum AEnumProp { get => _aEnumProp ?? throw new UnsetFeatureException(BLangLanguage.Instance.BConcept_AEnumProp); set => SetAEnumProp(value); }

	/// enum desc
    	/// <seealso cref = "LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.ALangLanguage"/>
    	/// <seealso cref = "LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept.BRef"/>
    	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public bool TryGetAEnumProp([MaybeNullWhenAttribute(false)] out LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AEnum? aEnumProp)
	{
		aEnumProp = _aEnumProp;
		return _aEnumProp != null;
	}

	/// enum desc
    	/// <seealso cref = "LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.ALangLanguage"/>
    	/// <seealso cref = "LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept.BRef"/>
    	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public BConcept SetAEnumProp(LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AEnum value)
	{
		AssureNotNull(value, BLangLanguage.Instance.BConcept_AEnumProp);
		_aEnumProp = value;
		return this;
	}

	private LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept? _aRef = null;
	/// <remarks>Optional Single Reference</remarks>
        [LionCoreMetaPointer(Language = typeof(BLangLanguage), Key = "key-ARef")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Reference, Optional = true, Multiple = true)]
	public LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept? ARef { get => _aRef; set => SetARef(value); }

	/// <remarks>Optional Single Reference</remarks>
        public bool TryGetARef([MaybeNullWhenAttribute(false)] out LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept? aRef)
	{
		aRef = _aRef;
		return _aRef != null;
	}

	/// <remarks>Optional Single Reference</remarks>
        public BConcept SetARef(LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept? value)
	{
		_aRef = value;
		return this;
	}

	public BConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => BLangLanguage.Instance.BConcept;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (BLangLanguage.Instance.BConcept_AEnumProp.EqualsIdentity(feature))
		{
			result = AEnumProp;
			return true;
		}

		if (BLangLanguage.Instance.BConcept_ARef.EqualsIdentity(feature))
		{
			result = ARef;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (BLangLanguage.Instance.BConcept_AEnumProp.EqualsIdentity(feature))
		{
			if (value is LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AEnum v)
			{
				AEnumProp = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (BLangLanguage.Instance.BConcept_ARef.EqualsIdentity(feature))
		{
			if (value is null or LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept)
			{
				ARef = (LionWeb.Core.Test.Languages.Generated.V2024_1.Circular.A.AConcept?)value;
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
		if (TryGetAEnumProp(out _))
			result.Add(BLangLanguage.Instance.BConcept_AEnumProp);
		if (TryGetARef(out _))
			result.Add(BLangLanguage.Instance.BConcept_ARef);
		return result;
	}
}