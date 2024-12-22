// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Examples.V2023_1.TinyRefLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2023_1;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-tinyRefLang", Version = "0")]
public partial class TinyRefLangLanguage : LanguageBase<ITinyRefLangFactory>
{
	public static readonly TinyRefLangLanguage Instance = new Lazy<TinyRefLangLanguage>(() => new("id-TinyRefLang")).Value;
	public TinyRefLangLanguage(string id) : base(id, LionWebVersions.v2023_1)
	{
		_myConcept = new(() => new ConceptBase<TinyRefLangLanguage>("id-Concept", this) { Key = "key-MyConcept", Name = "MyConcept", Abstract = false, Partition = false, FeaturesLazy = new(() => [MyConcept_multivaluedRef, MyConcept_singularRef]) });
		_myConcept_multivaluedRef = new(() => new ReferenceBase<TinyRefLangLanguage>("id-Concept-multivaluedRef", MyConcept, this) { Key = "key-MyConcept-multivaluedRef", Name = "multivaluedRef", Optional = false, Multiple = true, Type = MyConcept });
		_myConcept_singularRef = new(() => new ReferenceBase<TinyRefLangLanguage>("id-MyConcept-singularRef", MyConcept, this) { Key = "key-MyConcept-singularRef", Name = "singularRef", Optional = false, Multiple = false, Type = MyConcept });
		_factory = new TinyRefLangFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [MyConcept];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	private const string _key = "key-tinyRefLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "TinyRefLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "0";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _myConcept;
	public Concept MyConcept => _myConcept.Value;

	private readonly Lazy<Reference> _myConcept_multivaluedRef;
	public Reference MyConcept_multivaluedRef => _myConcept_multivaluedRef.Value;

	private readonly Lazy<Reference> _myConcept_singularRef;
	public Reference MyConcept_singularRef => _myConcept_singularRef.Value;
}

public partial interface ITinyRefLangFactory : INodeFactory
{
	public MyConcept NewMyConcept(string id);
	public MyConcept CreateMyConcept();
}

public class TinyRefLangFactory : AbstractBaseNodeFactory, ITinyRefLangFactory
{
	private readonly TinyRefLangLanguage _language;
	public TinyRefLangFactory(TinyRefLangLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.MyConcept.EqualsIdentity(classifier))
			return NewMyConcept(id);
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

	public virtual MyConcept NewMyConcept(string id) => new(id);
	public virtual MyConcept CreateMyConcept() => NewMyConcept(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(TinyRefLangLanguage), Key = "key-MyConcept")]
public partial class MyConcept : NodeBase
{
	private readonly List<MyConcept> _multivaluedRef = [];
	/// <remarks>Required Multiple Reference</remarks>
    	/// <exception cref = "UnsetFeatureException">If MultivaluedRef is empty</exception>
        [LionCoreMetaPointer(Language = typeof(TinyRefLangLanguage), Key = "key-MyConcept-multivaluedRef")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Reference, Optional = false, Multiple = false)]
	public IReadOnlyList<MyConcept> MultivaluedRef { get => AsNonEmptyReadOnly(_multivaluedRef, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef); init => AddMultivaluedRef(value); }

	/// <remarks>Required Multiple Reference</remarks>
    	/// <exception cref = "InvalidValueException">If both MultivaluedRef and nodes are empty</exception>
        public MyConcept AddMultivaluedRef(IEnumerable<MyConcept> nodes)
	{
		var safeNodes = nodes?.ToList();
		AssureNotNull(safeNodes, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
		AssureNonEmpty(safeNodes, _multivaluedRef, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
		_multivaluedRef.AddRange(safeNodes);
		return this;
	}

	/// <remarks>Required Multiple Reference</remarks>
    	/// <exception cref = "InvalidValueException">If both MultivaluedRef and nodes are empty</exception>
    	/// <exception cref = "ArgumentOutOfRangeException">If index negative or greater than MultivaluedRef.Count</exception>
        public MyConcept InsertMultivaluedRef(int index, IEnumerable<MyConcept> nodes)
	{
		AssureInRange(index, _multivaluedRef);
		var safeNodes = nodes?.ToList();
		AssureNotNull(safeNodes, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
		AssureNonEmpty(safeNodes, _multivaluedRef, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
		_multivaluedRef.InsertRange(index, safeNodes);
		return this;
	}

	/// <remarks>Required Multiple Reference</remarks>
    	/// <exception cref = "InvalidValueException">If MultivaluedRef would be empty</exception>
        public MyConcept RemoveMultivaluedRef(IEnumerable<MyConcept> nodes)
	{
		var safeNodes = nodes?.ToList();
		AssureNotNull(safeNodes, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
		AssureNonEmpty(safeNodes, _multivaluedRef, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
		AssureNotClearing(safeNodes, _multivaluedRef, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
		RemoveAll(safeNodes, _multivaluedRef);
		return this;
	}

	private MyConcept? _singularRef = null;
	/// <remarks>Required Single Reference</remarks>
    	/// <exception cref = "UnsetFeatureException">If SingularRef has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(TinyRefLangLanguage), Key = "key-MyConcept-singularRef")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Reference, Optional = false, Multiple = false)]
	public MyConcept SingularRef { get => _singularRef ?? throw new UnsetFeatureException(TinyRefLangLanguage.Instance.MyConcept_singularRef); set => SetSingularRef(value); }

	/// <remarks>Required Single Reference</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public MyConcept SetSingularRef(MyConcept value)
	{
		AssureNotNull(value, TinyRefLangLanguage.Instance.MyConcept_singularRef);
		_singularRef = value;
		return this;
	}

	public MyConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Classifier GetClassifier() => TinyRefLangLanguage.Instance.MyConcept;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (TinyRefLangLanguage.Instance.MyConcept_multivaluedRef.EqualsIdentity(feature))
		{
			result = MultivaluedRef;
			return true;
		}

		if (TinyRefLangLanguage.Instance.MyConcept_singularRef.EqualsIdentity(feature))
		{
			result = SingularRef;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (TinyRefLangLanguage.Instance.MyConcept_multivaluedRef.EqualsIdentity(feature))
		{
			var enumerable = TinyRefLangLanguage.Instance.MyConcept_multivaluedRef.AsNodes<Examples.V2023_1.TinyRefLang.MyConcept>(value).ToList();
			AssureNonEmpty(enumerable, TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
			_multivaluedRef.Clear();
			AddMultivaluedRef(enumerable);
			return true;
		}

		if (TinyRefLangLanguage.Instance.MyConcept_singularRef.EqualsIdentity(feature))
		{
			if (value is Examples.V2023_1.TinyRefLang.MyConcept v)
			{
				SingularRef = v;
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
		if (_multivaluedRef.Count != 0)
			result.Add(TinyRefLangLanguage.Instance.MyConcept_multivaluedRef);
		if (_singularRef != default)
			result.Add(TinyRefLangLanguage.Instance.MyConcept_singularRef);
		return result;
	}
}