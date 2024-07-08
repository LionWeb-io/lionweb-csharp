// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Examples.Cirular.A;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-ALang", Version = "1")]
public class ALangLanguage : LanguageBase<IALangFactory>
{
	public static readonly ALangLanguage Instance = new Lazy<ALangLanguage>(() => new("id-ALang")).Value;
	public ALangLanguage(string id) : base(id)
	{
		_aConcept = new(() => new ConceptBase<ALangLanguage>("id-AConcept", this) { Key = "key-AConcept", Name = "AConcept", Abstract = false, Partition = false, FeaturesLazy = new(() => [AConcept_BRef]) });
		_aConcept_BRef = new(() => new ReferenceBase<ALangLanguage>("id-AConcept-BRef", AConcept, this) { Key = "key-BRef", Name = "BRef", Optional = true, Multiple = false, Type = Examples.Cirular.B.BLangLanguage.Instance.BConcept });
		_aEnum = new(() => new EnumerationBase<ALangLanguage>("id-aEnum", this) { Key = "key-AEnum", Name = "AEnum", LiteralsLazy = new(() => [AEnum_left, AEnum_right]) });
		_aEnum_left = new(() => new EnumerationLiteralBase<ALangLanguage>("id-left", AEnum, this) { Key = "key-left", Name = "left" });
		_aEnum_right = new(() => new EnumerationLiteralBase<ALangLanguage>("id-right", AEnum, this) { Key = "key-right", Name = "right" });
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [AConcept, AEnum];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [Examples.Cirular.B.BLangLanguage.Instance];

	/// <inheritdoc/>
        public override ALangFactory GetFactory() => new(this);
	private const string _key = "key-ALang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "ALang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _aConcept;
	public Concept AConcept => _aConcept.Value;

	private readonly Lazy<Reference> _aConcept_BRef;
	public Reference AConcept_BRef => _aConcept_BRef.Value;

	private readonly Lazy<Enumeration> _aEnum;
	public Enumeration AEnum => _aEnum.Value;

	private readonly Lazy<EnumerationLiteral> _aEnum_left;
	public EnumerationLiteral AEnum_left => _aEnum_left.Value;

	private readonly Lazy<EnumerationLiteral> _aEnum_right;
	public EnumerationLiteral AEnum_right => _aEnum_right.Value;
}

public interface IALangFactory : INodeFactory
{
    public AConcept NewAConcept(string id);
    public AConcept CreateAConcept();
}

public class ALangFactory : AbstractBaseNodeFactory, IALangFactory
{
	private readonly ALangLanguage _language;
	public ALangFactory(ALangLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.AConcept.EqualsIdentity(classifier))
			return NewAConcept(id);
		throw new UnsupportedClassifierException(classifier);
	}

	/// <inheritdoc/>
        public override Enum GetEnumerationLiteral(EnumerationLiteral literal)
	{
		if (_language.AEnum.EqualsIdentity(literal.GetEnumeration()))
			return EnumValueFor<AEnum>(literal);
		throw new UnsupportedEnumerationLiteralException(literal);
	}

	public AConcept NewAConcept(string id) => new(id);
	public AConcept CreateAConcept() => NewAConcept(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(ALangLanguage), Key = "key-AConcept")]
public class AConcept : NodeBase
{
	public AConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Classifier GetClassifier() => ALangLanguage.Instance.AConcept;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (ALangLanguage.Instance.AConcept_BRef.EqualsIdentity(feature))
		{
			result = BRef;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (ALangLanguage.Instance.AConcept_BRef.EqualsIdentity(feature))
		{
			if (value is null or Examples.Cirular.B.BConcept)
			{
				BRef = (Examples.Cirular.B.BConcept?)value;
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
		if (_bRef != default)
			result.Add(ALangLanguage.Instance.AConcept_BRef);
		return result;
	}

	private Examples.Cirular.B.BConcept? _bRef = null;
	/// <remarks>Optional Single Reference</remarks>
        [LionCoreMetaPointer(Language = typeof(ALangLanguage), Key = "key-BRef")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Reference, Optional = true, Multiple = true)]
	public Examples.Cirular.B.BConcept? BRef { get => _bRef; set => SetBRef(value); }

	/// <remarks>Optional Single Reference</remarks>
        public AConcept SetBRef(Examples.Cirular.B.BConcept? value)
	{
		_bRef = value;
		return this;
	}
}

[LionCoreMetaPointer(Language = typeof(ALangLanguage), Key = "key-AEnum")]
public enum AEnum
{
	[LionCoreMetaPointer(Language = typeof(ALangLanguage), Key = "key-left")]
	@left,
	[LionCoreMetaPointer(Language = typeof(ALangLanguage), Key = "key-right")]
	@right
}