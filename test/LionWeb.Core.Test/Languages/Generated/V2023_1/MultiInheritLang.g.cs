// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace LionWeb.Core.Test.Languages.Generated.V2023_1.MultiInheritLang;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2023_1;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "key-MultiInheritLang", Version = "1")]
public partial class MultiInheritLangLanguage : LanguageBase<IMultiInheritLangFactory>
{
	public static readonly MultiInheritLangLanguage Instance = new Lazy<MultiInheritLangLanguage>(() => new("id-MultiInheritLang")).Value;
	public MultiInheritLangLanguage(string id) : base(id, LionWebVersions.v2023_1)
	{
		_abstractConcept = new(() => new ConceptBase<MultiInheritLangLanguage>("id-AbstractConcept", this) { Key = "key-AbstractConcept", Name = "AbstractConcept", Abstract = true, Partition = false, ImplementsLazy = new(() => [BaseIface]) });
		_baseIface = new(() => new InterfaceBase<MultiInheritLangLanguage>("id-BaseIface", this) { Key = "key-BaseIface", Name = "BaseIface", FeaturesLazy = new(() => [BaseIface_ifaceContainment]) });
		_baseIface_ifaceContainment = new(() => new ContainmentBase<MultiInheritLangLanguage>("id-ifaceContainment", BaseIface, this) { Key = "key-ifaceContainment", Name = "ifaceContainment", Optional = false, Multiple = false, Type = _builtIns.Node });
		_combinedConcept = new(() => new ConceptBase<MultiInheritLangLanguage>("id-CombinedConcept", this) { Key = "key-CombinedConcept", Name = "CombinedConcept", Abstract = false, Partition = false, ExtendsLazy = new(() => IntermediateConcept), ImplementsLazy = new(() => [BaseIface]) });
		_intermediateConcept = new(() => new ConceptBase<MultiInheritLangLanguage>("id-IntermediateConcept", this) { Key = "key-IntermediateConcept", Name = "IntermediateConcept", Abstract = false, Partition = false, ExtendsLazy = new(() => AbstractConcept) });
		_factory = new MultiInheritLangFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [AbstractConcept, BaseIface, CombinedConcept, IntermediateConcept];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	private const string _key = "key-MultiInheritLang";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "MultiInheritLang";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _abstractConcept;
	public Concept AbstractConcept => _abstractConcept.Value;

	private readonly Lazy<Interface> _baseIface;
	public Interface BaseIface => _baseIface.Value;

	private readonly Lazy<Containment> _baseIface_ifaceContainment;
	public Containment BaseIface_ifaceContainment => _baseIface_ifaceContainment.Value;

	private readonly Lazy<Concept> _combinedConcept;
	public Concept CombinedConcept => _combinedConcept.Value;

	private readonly Lazy<Concept> _intermediateConcept;
	public Concept IntermediateConcept => _intermediateConcept.Value;
}

public partial interface IMultiInheritLangFactory : INodeFactory
{
	public CombinedConcept NewCombinedConcept(string id);
	public CombinedConcept CreateCombinedConcept();
	public IntermediateConcept NewIntermediateConcept(string id);
	public IntermediateConcept CreateIntermediateConcept();
}

public class MultiInheritLangFactory : AbstractBaseNodeFactory, IMultiInheritLangFactory
{
	private readonly MultiInheritLangLanguage _language;
	public MultiInheritLangFactory(MultiInheritLangLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.CombinedConcept.EqualsIdentity(classifier))
			return NewCombinedConcept(id);
		if (_language.IntermediateConcept.EqualsIdentity(classifier))
			return NewIntermediateConcept(id);
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

	public virtual CombinedConcept NewCombinedConcept(string id) => new(id);
	public virtual CombinedConcept CreateCombinedConcept() => NewCombinedConcept(GetNewId());
	public virtual IntermediateConcept NewIntermediateConcept(string id) => new(id);
	public virtual IntermediateConcept CreateIntermediateConcept() => NewIntermediateConcept(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(MultiInheritLangLanguage), Key = "key-AbstractConcept")]
public abstract partial class AbstractConcept : ConceptInstanceBase, BaseIface
{
	private NodeBase? _ifaceContainment = null;
	/// <remarks>Required Single Containment</remarks>
    	/// <exception cref = "UnsetFeatureException">If IfaceContainment has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(MultiInheritLangLanguage), Key = "key-ifaceContainment")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Containment, Optional = false, Multiple = false)]
	public NodeBase IfaceContainment { get => _ifaceContainment ?? throw new UnsetFeatureException(MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment); set => SetIfaceContainment(value); }
/// <remarks>Required Single Containment</remarks>
/// <exception cref="InvalidValueException">If set to null</exception>
 BaseIface BaseIface.SetIfaceContainment(NodeBase value) => SetIfaceContainment(value);
	/// <remarks>Required Single Containment</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public AbstractConcept SetIfaceContainment(NodeBase value)
	{
		AssureNotNull(value, MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment);
		SetParentNull(_ifaceContainment);
		AttachChild(value);
		_ifaceContainment = value;
		return this;
	}

	public AbstractConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => MultiInheritLangLanguage.Instance.AbstractConcept;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment.EqualsIdentity(feature))
		{
			result = IfaceContainment;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment.EqualsIdentity(feature))
		{
			if (value is NodeBase v)
			{
				IfaceContainment = v;
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
		if (_ifaceContainment != default)
			result.Add(MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment);
		return result;
	}

	/// <inheritdoc/>
        protected override bool DetachChild(INode child)
	{
		if (base.DetachChild(child))
			return true;
		Containment? c = GetContainmentOf(child);
		if (MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment.EqualsIdentity(c))
		{
			_ifaceContainment = null;
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
		if (ReferenceEquals(_ifaceContainment, child))
			return MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment;
		return null;
	}
}

[LionCoreMetaPointer(Language = typeof(MultiInheritLangLanguage), Key = "key-BaseIface")]
public partial interface BaseIface : INode
{
	/// <remarks>Required Single Containment</remarks>
        [LionCoreMetaPointer(Language = typeof(MultiInheritLangLanguage), Key = "key-ifaceContainment")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Containment, Optional = false, Multiple = false)]
	public NodeBase IfaceContainment { get; set; }

	/// <remarks>Required Single Containment</remarks>
        public BaseIface SetIfaceContainment(NodeBase value);
}

[LionCoreMetaPointer(Language = typeof(MultiInheritLangLanguage), Key = "key-CombinedConcept")]
public partial class CombinedConcept : IntermediateConcept, BaseIface
{
	public CombinedConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => MultiInheritLangLanguage.Instance.CombinedConcept;
}

[LionCoreMetaPointer(Language = typeof(MultiInheritLangLanguage), Key = "key-IntermediateConcept")]
public partial class IntermediateConcept : AbstractConcept
{
	public IntermediateConcept(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => MultiInheritLangLanguage.Instance.IntermediateConcept;
}