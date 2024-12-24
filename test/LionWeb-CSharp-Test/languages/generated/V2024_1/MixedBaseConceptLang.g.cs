// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Examples.V2024_1.Mixed.MixedBaseConceptLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2024_1;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-mixedBaseConceptLang", Version = "1")]
public partial class MixedBaseConceptLangLanguage : LanguageBase<IMixedBaseConceptLangFactory>
{
	public static readonly MixedBaseConceptLangLanguage Instance = new Lazy<MixedBaseConceptLangLanguage>(() => new("id-mixedBaseConceptLang")).Value;
	public MixedBaseConceptLangLanguage(string id) : base(id, LionWebVersions.v2024_1)
	{
		_baseConcept = new(() => new ConceptBase<MixedBaseConceptLangLanguage>("id-baseConcept", this) { Key = "key-baseConcept", Name = "BaseConcept", Abstract = true, Partition = false, ImplementsLazy = new(() => [Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance.BaseContainmentIface, Examples.V2024_1.Mixed.MixedBasePropertyLang.MixedBasePropertyLangLanguage.Instance.BasePropertyIface, Examples.V2024_1.Mixed.MixedBaseReferenceLang.MixedBaseReferenceLangLanguage.Instance.BaseReferenceIface]), FeaturesLazy = new(() => [BaseConcept_enumProp, BaseConcept_sdtProp]) });
		_baseConcept_enumProp = new(() => new PropertyBase<MixedBaseConceptLangLanguage>("id-enumProp", BaseConcept, this) { Key = "key-enumProp", Name = "enumProp", Optional = false, Type = Examples.V2024_1.Mixed.MixedDirectEnumLang.MixedDirectEnumLangLanguage.Instance.DirectEnum });
		_baseConcept_sdtProp = new(() => new PropertyBase<MixedBaseConceptLangLanguage>("id-sdtProp", BaseConcept, this) { Key = "key-sdtProp", Name = "sdtProp", Optional = false, Type = Examples.V2024_1.Mixed.MixedDirectSdtLang.MixedDirectSdtLangLanguage.Instance.DirectSdt });
		_factory = new MixedBaseConceptLangFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [BaseConcept];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance, Examples.V2024_1.Mixed.MixedBasePropertyLang.MixedBasePropertyLangLanguage.Instance, Examples.V2024_1.Mixed.MixedBaseReferenceLang.MixedBaseReferenceLangLanguage.Instance, Examples.V2024_1.Mixed.MixedDirectEnumLang.MixedDirectEnumLangLanguage.Instance, Examples.V2024_1.Mixed.MixedDirectSdtLang.MixedDirectSdtLangLanguage.Instance];

	private const string _key = "key-mixedBaseConceptLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "MixedBaseConceptLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _baseConcept;
	public Concept BaseConcept => _baseConcept.Value;

	private readonly Lazy<Property> _baseConcept_enumProp;
	public Property BaseConcept_enumProp => _baseConcept_enumProp.Value;

	private readonly Lazy<Property> _baseConcept_sdtProp;
	public Property BaseConcept_sdtProp => _baseConcept_sdtProp.Value;
}

public partial interface IMixedBaseConceptLangFactory : INodeFactory
{
}

public class MixedBaseConceptLangFactory : AbstractBaseNodeFactory, IMixedBaseConceptLangFactory
{
	private readonly MixedBaseConceptLangLanguage _language;
	public MixedBaseConceptLangFactory(MixedBaseConceptLangLanguage language) : base(language)
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

[LionCoreMetaPointer(Language = typeof(MixedBaseConceptLangLanguage), Key = "key-baseConcept")]
public abstract partial class BaseConcept : NodeBase, Examples.V2024_1.Mixed.MixedBaseContainmentLang.BaseContainmentIface, Examples.V2024_1.Mixed.MixedBasePropertyLang.BasePropertyIface, Examples.V2024_1.Mixed.MixedBaseReferenceLang.BaseReferenceIface
{
	private Examples.V2024_1.Mixed.MixedDirectEnumLang.DirectEnum? _enumProp = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If EnumProp has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(MixedBaseConceptLangLanguage), Key = "key-enumProp")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public Examples.V2024_1.Mixed.MixedDirectEnumLang.DirectEnum EnumProp { get => _enumProp ?? throw new UnsetFeatureException(MixedBaseConceptLangLanguage.Instance.BaseConcept_enumProp); set => SetEnumProp(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public BaseConcept SetEnumProp(Examples.V2024_1.Mixed.MixedDirectEnumLang.DirectEnum value)
	{
		AssureNotNull(value, MixedBaseConceptLangLanguage.Instance.BaseConcept_enumProp);
		_enumProp = value;
		return this;
	}

	private Examples.V2024_1.Mixed.MixedDirectSdtLang.DirectSdt? _sdtProp = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If SdtProp has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(MixedBaseConceptLangLanguage), Key = "key-sdtProp")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public Examples.V2024_1.Mixed.MixedDirectSdtLang.DirectSdt SdtProp { get => _sdtProp ?? throw new UnsetFeatureException(MixedBaseConceptLangLanguage.Instance.BaseConcept_sdtProp); set => SetSdtProp(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public BaseConcept SetSdtProp(Examples.V2024_1.Mixed.MixedDirectSdtLang.DirectSdt value)
	{
		AssureNotNull(value, MixedBaseConceptLangLanguage.Instance.BaseConcept_sdtProp);
		_sdtProp = value;
		return this;
	}

	private NodeBase? _cont = null;
	/// <remarks>Required Single Containment</remarks>
    	/// <exception cref = "UnsetFeatureException">If Cont has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage), Key = "key-baseContainmentIface-cont")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Containment, Optional = false, Multiple = false)]
	public NodeBase Cont { get => _cont ?? throw new UnsetFeatureException(Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance.BaseContainmentIface_Cont); set => SetCont(value); }
/// <remarks>Required Single Containment</remarks>
/// <exception cref="InvalidValueException">If set to null</exception>
 Examples.V2024_1.Mixed.MixedBaseContainmentLang.BaseContainmentIface Examples.V2024_1.Mixed.MixedBaseContainmentLang.BaseContainmentIface.SetCont(NodeBase value) => SetCont(value);
	/// <remarks>Required Single Containment</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public BaseConcept SetCont(NodeBase value)
	{
		AssureNotNull(value, Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance.BaseContainmentIface_Cont);
		SetParentNull(_cont);
		AttachChild(value);
		_cont = value;
		return this;
	}

	private string? _prop = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Prop has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(Examples.V2024_1.Mixed.MixedBasePropertyLang.MixedBasePropertyLangLanguage), Key = "key-basePropertyIface-prop")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public string Prop { get => _prop ?? throw new UnsetFeatureException(Examples.V2024_1.Mixed.MixedBasePropertyLang.MixedBasePropertyLangLanguage.Instance.BasePropertyIface_Prop); set => SetProp(value); }
/// <remarks>Required Property</remarks>
/// <exception cref="InvalidValueException">If set to null</exception>
 Examples.V2024_1.Mixed.MixedBasePropertyLang.BasePropertyIface Examples.V2024_1.Mixed.MixedBasePropertyLang.BasePropertyIface.SetProp(string value) => SetProp(value);
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public BaseConcept SetProp(string value)
	{
		AssureNotNull(value, Examples.V2024_1.Mixed.MixedBasePropertyLang.MixedBasePropertyLangLanguage.Instance.BasePropertyIface_Prop);
		_prop = value;
		return this;
	}

	private NodeBase? _ref = null;
	/// <remarks>Required Single Reference</remarks>
    	/// <exception cref = "UnsetFeatureException">If Ref has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(Examples.V2024_1.Mixed.MixedBaseReferenceLang.MixedBaseReferenceLangLanguage), Key = "key-baseReferenceIface-ref")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Reference, Optional = false, Multiple = false)]
	public NodeBase Ref { get => _ref ?? throw new UnsetFeatureException(Examples.V2024_1.Mixed.MixedBaseReferenceLang.MixedBaseReferenceLangLanguage.Instance.BaseReferenceIface_Ref); set => SetRef(value); }
/// <remarks>Required Single Reference</remarks>
/// <exception cref="InvalidValueException">If set to null</exception>
 Examples.V2024_1.Mixed.MixedBaseReferenceLang.BaseReferenceIface Examples.V2024_1.Mixed.MixedBaseReferenceLang.BaseReferenceIface.SetRef(NodeBase value) => SetRef(value);
	/// <remarks>Required Single Reference</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public BaseConcept SetRef(NodeBase value)
	{
		AssureNotNull(value, Examples.V2024_1.Mixed.MixedBaseReferenceLang.MixedBaseReferenceLangLanguage.Instance.BaseReferenceIface_Ref);
		_ref = value;
		return this;
	}

	public BaseConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Classifier GetClassifier() => MixedBaseConceptLangLanguage.Instance.BaseConcept;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (MixedBaseConceptLangLanguage.Instance.BaseConcept_enumProp.EqualsIdentity(feature))
		{
			result = EnumProp;
			return true;
		}

		if (MixedBaseConceptLangLanguage.Instance.BaseConcept_sdtProp.EqualsIdentity(feature))
		{
			result = SdtProp;
			return true;
		}

		if (Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance.BaseContainmentIface_Cont.EqualsIdentity(feature))
		{
			result = Cont;
			return true;
		}

		if (Examples.V2024_1.Mixed.MixedBasePropertyLang.MixedBasePropertyLangLanguage.Instance.BasePropertyIface_Prop.EqualsIdentity(feature))
		{
			result = Prop;
			return true;
		}

		if (Examples.V2024_1.Mixed.MixedBaseReferenceLang.MixedBaseReferenceLangLanguage.Instance.BaseReferenceIface_Ref.EqualsIdentity(feature))
		{
			result = Ref;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (MixedBaseConceptLangLanguage.Instance.BaseConcept_enumProp.EqualsIdentity(feature))
		{
			if (value is Examples.V2024_1.Mixed.MixedDirectEnumLang.DirectEnum v)
			{
				EnumProp = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (MixedBaseConceptLangLanguage.Instance.BaseConcept_sdtProp.EqualsIdentity(feature))
		{
			if (value is Examples.V2024_1.Mixed.MixedDirectSdtLang.DirectSdt v)
			{
				SdtProp = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance.BaseContainmentIface_Cont.EqualsIdentity(feature))
		{
			if (value is NodeBase v)
			{
				Cont = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (Examples.V2024_1.Mixed.MixedBasePropertyLang.MixedBasePropertyLangLanguage.Instance.BasePropertyIface_Prop.EqualsIdentity(feature))
		{
			if (value is string v)
			{
				Prop = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (Examples.V2024_1.Mixed.MixedBaseReferenceLang.MixedBaseReferenceLangLanguage.Instance.BaseReferenceIface_Ref.EqualsIdentity(feature))
		{
			if (value is NodeBase v)
			{
				Ref = v;
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
		if (_enumProp != default)
			result.Add(MixedBaseConceptLangLanguage.Instance.BaseConcept_enumProp);
		if (_sdtProp != default)
			result.Add(MixedBaseConceptLangLanguage.Instance.BaseConcept_sdtProp);
		if (_cont != default)
			result.Add(Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance.BaseContainmentIface_Cont);
		if (_prop != default)
			result.Add(Examples.V2024_1.Mixed.MixedBasePropertyLang.MixedBasePropertyLangLanguage.Instance.BasePropertyIface_Prop);
		if (_ref != default)
			result.Add(Examples.V2024_1.Mixed.MixedBaseReferenceLang.MixedBaseReferenceLangLanguage.Instance.BaseReferenceIface_Ref);
		return result;
	}

	/// <inheritdoc/>
        protected override bool DetachChild(INode child)
	{
		if (base.DetachChild(child))
			return true;
		Containment? c = GetContainmentOf(child);
		if (Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance.BaseContainmentIface_Cont.EqualsIdentity(c))
		{
			_cont = null;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        public override Containment? GetContainmentOf(INode child)
	{
		Containment? result = base.GetContainmentOf(child);
		if (result != null)
			return result;
		if (ReferenceEquals(_cont, child))
			return Examples.V2024_1.Mixed.MixedBaseContainmentLang.MixedBaseContainmentLangLanguage.Instance.BaseContainmentIface_Cont;
		return null;
	}
}