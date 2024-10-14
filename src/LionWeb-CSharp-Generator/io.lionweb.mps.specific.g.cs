// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace Io.Lionweb.Mps.Specific;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using System;
using System.Collections.Generic;

[LionCoreLanguage(Key = "io-lionweb-mps-specific", Version = "0")]
public class SpecificLanguage : LanguageBase<ISpecificFactory>
{
	public static readonly SpecificLanguage Instance = new Lazy<SpecificLanguage>(() => new("io-lionweb-mps-specific")).Value;
	public SpecificLanguage(string id) : base(id)
	{
		_conceptDescription = new(() => new AnnotationBase<SpecificLanguage>("ConceptDescription", this) { Key = "ConceptDescription", Name = "ConceptDescription", AnnotatesLazy = new(() => M3Language.Instance.Classifier), FeaturesLazy = new(() => [ConceptDescription_conceptAlias, ConceptDescription_conceptShortDescription]) });
		_conceptDescription_conceptAlias = new(() => new PropertyBase<SpecificLanguage>("ConceptDescription-conceptAlias", ConceptDescription, this) { Key = "ConceptDescription-conceptAlias", Name = "conceptAlias", Optional = true, Type = BuiltInsLanguage.Instance.String });
		_conceptDescription_conceptShortDescription = new(() => new PropertyBase<SpecificLanguage>("ConceptDescription-conceptShortDescription", ConceptDescription, this) { Key = "ConceptDescription-conceptShortDescription", Name = "conceptShortDescription", Optional = true, Type = BuiltInsLanguage.Instance.String });
		_deprecated = new(() => new AnnotationBase<SpecificLanguage>("Deprecated", this) { Key = "Deprecated", Name = "Deprecated", AnnotatesLazy = new(() => M3Language.Instance.IKeyed), FeaturesLazy = new(() => [Deprecated_build, Deprecated_comment]) });
		_deprecated_build = new(() => new PropertyBase<SpecificLanguage>("Deprecated-build", Deprecated, this) { Key = "Deprecated-build", Name = "build", Optional = true, Type = BuiltInsLanguage.Instance.String });
		_deprecated_comment = new(() => new PropertyBase<SpecificLanguage>("Deprecated-comment", Deprecated, this) { Key = "Deprecated-comment", Name = "comment", Optional = true, Type = BuiltInsLanguage.Instance.String });
		_shortDescription = new(() => new AnnotationBase<SpecificLanguage>("ShortDescription", this) { Key = "ShortDescription", Name = "ShortDescription", AnnotatesLazy = new(() => BuiltInsLanguage.Instance.Node), FeaturesLazy = new(() => [ShortDescription_description]) });
		_shortDescription_description = new(() => new PropertyBase<SpecificLanguage>("ShortDescription-description", ShortDescription, this) { Key = "ShortDescription-description", Name = "description", Optional = true, Type = BuiltInsLanguage.Instance.String });
		_virtualPackage = new(() => new AnnotationBase<SpecificLanguage>("VirtualPackage", this) { Key = "VirtualPackage", Name = "VirtualPackage", AnnotatesLazy = new(() => BuiltInsLanguage.Instance.Node), ImplementsLazy = new(() => [BuiltInsLanguage.Instance.INamed]) });
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [ConceptDescription, Deprecated, ShortDescription, VirtualPackage];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	/// <inheritdoc/>
        public override ISpecificFactory GetFactory() => new SpecificFactory(this);
	private const string _key = "io-lionweb-mps-specific";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "io.lionweb.mps.specific";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "0";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Annotation> _conceptDescription;
	public Annotation ConceptDescription => _conceptDescription.Value;

	private readonly Lazy<Property> _conceptDescription_conceptAlias;
	public Property ConceptDescription_conceptAlias => _conceptDescription_conceptAlias.Value;

	private readonly Lazy<Property> _conceptDescription_conceptShortDescription;
	public Property ConceptDescription_conceptShortDescription => _conceptDescription_conceptShortDescription.Value;

	private readonly Lazy<Annotation> _deprecated;
	public Annotation Deprecated => _deprecated.Value;

	private readonly Lazy<Property> _deprecated_build;
	public Property Deprecated_build => _deprecated_build.Value;

	private readonly Lazy<Property> _deprecated_comment;
	public Property Deprecated_comment => _deprecated_comment.Value;

	private readonly Lazy<Annotation> _shortDescription;
	public Annotation ShortDescription => _shortDescription.Value;

	private readonly Lazy<Property> _shortDescription_description;
	public Property ShortDescription_description => _shortDescription_description.Value;

	private readonly Lazy<Annotation> _virtualPackage;
	public Annotation VirtualPackage => _virtualPackage.Value;
}

public interface ISpecificFactory : INodeFactory
{
	public ConceptDescription NewConceptDescription(string id);
	public ConceptDescription CreateConceptDescription();
	public Deprecated NewDeprecated(string id);
	public Deprecated CreateDeprecated();
	public ShortDescription NewShortDescription(string id);
	public ShortDescription CreateShortDescription();
	public VirtualPackage NewVirtualPackage(string id);
	public VirtualPackage CreateVirtualPackage();
}

public class SpecificFactory : AbstractBaseNodeFactory, ISpecificFactory
{
	private readonly SpecificLanguage _language;
	public SpecificFactory(SpecificLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.ConceptDescription.EqualsIdentity(classifier))
			return NewConceptDescription(id);
		if (_language.Deprecated.EqualsIdentity(classifier))
			return NewDeprecated(id);
		if (_language.ShortDescription.EqualsIdentity(classifier))
			return NewShortDescription(id);
		if (_language.VirtualPackage.EqualsIdentity(classifier))
			return NewVirtualPackage(id);
		throw new UnsupportedClassifierException(classifier);
	}

	/// <inheritdoc/>
        public override Enum GetEnumerationLiteral(EnumerationLiteral literal)
	{
		throw new UnsupportedEnumerationLiteralException(literal);
	}

	public virtual ConceptDescription NewConceptDescription(string id) => new(id);
	public virtual ConceptDescription CreateConceptDescription() => NewConceptDescription(GetNewId());
	public virtual Deprecated NewDeprecated(string id) => new(id);
	public virtual Deprecated CreateDeprecated() => NewDeprecated(GetNewId());
	public virtual ShortDescription NewShortDescription(string id) => new(id);
	public virtual ShortDescription CreateShortDescription() => NewShortDescription(GetNewId());
	public virtual VirtualPackage NewVirtualPackage(string id) => new(id);
	public virtual VirtualPackage CreateVirtualPackage() => NewVirtualPackage(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "ConceptDescription")]
public class ConceptDescription : NodeBase
{
	private string? _conceptAlias = null;
	/// <remarks>Optional Property</remarks>
        [LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "ConceptDescription-conceptAlias")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = true, Multiple = false)]
	public string? ConceptAlias { get => _conceptAlias; set => SetConceptAlias(value); }

	/// <remarks>Optional Property</remarks>
        public ConceptDescription SetConceptAlias(string? value)
	{
		_conceptAlias = value;
		return this;
	}

	private string? _conceptShortDescription = null;
	/// <remarks>Optional Property</remarks>
        [LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "ConceptDescription-conceptShortDescription")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = true, Multiple = false)]
	public string? ConceptShortDescription { get => _conceptShortDescription; set => SetConceptShortDescription(value); }

	/// <remarks>Optional Property</remarks>
        public ConceptDescription SetConceptShortDescription(string? value)
	{
		_conceptShortDescription = value;
		return this;
	}

	public ConceptDescription(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Classifier GetClassifier() => SpecificLanguage.Instance.ConceptDescription;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (SpecificLanguage.Instance.ConceptDescription_conceptAlias.EqualsIdentity(feature))
		{
			result = ConceptAlias;
			return true;
		}

		if (SpecificLanguage.Instance.ConceptDescription_conceptShortDescription.EqualsIdentity(feature))
		{
			result = ConceptShortDescription;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (SpecificLanguage.Instance.ConceptDescription_conceptAlias.EqualsIdentity(feature))
		{
			if (value is null or string)
			{
				ConceptAlias = (string?)value;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (SpecificLanguage.Instance.ConceptDescription_conceptShortDescription.EqualsIdentity(feature))
		{
			if (value is null or string)
			{
				ConceptShortDescription = (string?)value;
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
		if (_conceptAlias != default)
			result.Add(SpecificLanguage.Instance.ConceptDescription_conceptAlias);
		if (_conceptShortDescription != default)
			result.Add(SpecificLanguage.Instance.ConceptDescription_conceptShortDescription);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "Deprecated")]
public class Deprecated : NodeBase
{
	private string? _build = null;
	/// <remarks>Optional Property</remarks>
        [LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "Deprecated-build")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = true, Multiple = false)]
	public string? Build { get => _build; set => SetBuild(value); }

	/// <remarks>Optional Property</remarks>
        public Deprecated SetBuild(string? value)
	{
		_build = value;
		return this;
	}

	private string? _comment = null;
	/// <remarks>Optional Property</remarks>
        [LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "Deprecated-comment")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = true, Multiple = false)]
	public string? Comment { get => _comment; set => SetComment(value); }

	/// <remarks>Optional Property</remarks>
        public Deprecated SetComment(string? value)
	{
		_comment = value;
		return this;
	}

	public Deprecated(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Classifier GetClassifier() => SpecificLanguage.Instance.Deprecated;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (SpecificLanguage.Instance.Deprecated_build.EqualsIdentity(feature))
		{
			result = Build;
			return true;
		}

		if (SpecificLanguage.Instance.Deprecated_comment.EqualsIdentity(feature))
		{
			result = Comment;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (SpecificLanguage.Instance.Deprecated_build.EqualsIdentity(feature))
		{
			if (value is null or string)
			{
				Build = (string?)value;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (SpecificLanguage.Instance.Deprecated_comment.EqualsIdentity(feature))
		{
			if (value is null or string)
			{
				Comment = (string?)value;
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
		if (_build != default)
			result.Add(SpecificLanguage.Instance.Deprecated_build);
		if (_comment != default)
			result.Add(SpecificLanguage.Instance.Deprecated_comment);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "ShortDescription")]
public class ShortDescription : NodeBase
{
	private string? _description = null;
	/// <remarks>Optional Property</remarks>
        [LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "ShortDescription-description")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = true, Multiple = false)]
	public string? Description { get => _description; set => SetDescription(value); }

	/// <remarks>Optional Property</remarks>
        public ShortDescription SetDescription(string? value)
	{
		_description = value;
		return this;
	}

	public ShortDescription(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Classifier GetClassifier() => SpecificLanguage.Instance.ShortDescription;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (SpecificLanguage.Instance.ShortDescription_description.EqualsIdentity(feature))
		{
			result = Description;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (SpecificLanguage.Instance.ShortDescription_description.EqualsIdentity(feature))
		{
			if (value is null or string)
			{
				Description = (string?)value;
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
		if (_description != default)
			result.Add(SpecificLanguage.Instance.ShortDescription_description);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(SpecificLanguage), Key = "VirtualPackage")]
public class VirtualPackage : NodeBase, INamedWritable
{
	private string? _name = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Name has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(BuiltInsLanguage), Key = "LionCore-builtins-INamed-name")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public string Name { get => _name ?? throw new UnsetFeatureException(BuiltInsLanguage.Instance.INamed_name); set => SetName(value); }
/// <remarks>Required Property</remarks>
/// <exception cref="InvalidValueException">If set to null</exception>
 INamedWritable INamedWritable.SetName(string value) => SetName(value);
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public VirtualPackage SetName(string value)
	{
		AssureNotNull(value, BuiltInsLanguage.Instance.INamed_name);
		_name = value;
		return this;
	}

	public VirtualPackage(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Classifier GetClassifier() => SpecificLanguage.Instance.VirtualPackage;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (BuiltInsLanguage.Instance.INamed_name.EqualsIdentity(feature))
		{
			result = Name;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (BuiltInsLanguage.Instance.INamed_name.EqualsIdentity(feature))
		{
			if (value is string v)
			{
				Name = v;
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
		if (_name != default)
			result.Add(BuiltInsLanguage.Instance.INamed_name);
		return result;
	}
}