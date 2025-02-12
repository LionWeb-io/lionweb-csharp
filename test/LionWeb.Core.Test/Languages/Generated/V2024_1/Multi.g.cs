// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace LionWeb.Core.Test.Languages.Generated.V2024_1.Multi.M2;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2024_1;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[LionCoreLanguage(Key = "multi", Version = "1")]
public partial class MultiLanguage : LanguageBase<IMultiFactory>
{
	public static readonly MultiLanguage Instance = new Lazy<MultiLanguage>(() => new("S9d8_7DajL2oOe1cqvXUGPGa3cWUF3bocmHgANb5bpM")).Value;
	public MultiLanguage(string id) : base(id, LionWebVersions.v2024_1)
	{
		_container = new(() => new ConceptBase<MultiLanguage>("qc3OObx7WI52EBnUJrrzWtbH9d3mXI7V6gTag7FZx0o", this) { Key = "Container", Name = "Container", Abstract = false, Partition = false, FeaturesLazy = new(() => [Container_libraries]) });
		_container_libraries = new(() => new ContainmentBase<MultiLanguage>("60eh9gc18v-vyPc8t3zNGWKgKPQ8Pmu85RmAoWlSV8U", Container, this) { Key = "libraries", Name = "libraries", Optional = false, Multiple = true, Type = LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.LibraryLanguage.Instance.Library });
		_factory = new MultiFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [Container];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.LibraryLanguage.Instance];

	private const string _key = "multi";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "multi";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _container;
	public Concept Container => _container.Value;

	private readonly Lazy<Containment> _container_libraries;
	public Containment Container_libraries => _container_libraries.Value;
}

public partial interface IMultiFactory : INodeFactory
{
	public Container NewContainer(string id);
	public Container CreateContainer();
}

public class MultiFactory : AbstractBaseNodeFactory, IMultiFactory
{
	private readonly MultiLanguage _language;
	public MultiFactory(MultiLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.Container.EqualsIdentity(classifier))
			return NewContainer(id);
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

	public virtual Container NewContainer(string id) => new(id);
	public virtual Container CreateContainer() => NewContainer(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(MultiLanguage), Key = "Container")]
public partial class Container : ConceptInstanceBase
{
	private readonly List<LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Library> _libraries = [];
	/// <remarks>Required Multiple Containment</remarks>
    	/// <exception cref = "UnsetFeatureException">If Libraries is empty</exception>
        [LionCoreMetaPointer(Language = typeof(MultiLanguage), Key = "libraries")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Containment, Optional = false, Multiple = false)]
	public IReadOnlyList<LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Library> Libraries { get => AsNonEmptyReadOnly(_libraries, MultiLanguage.Instance.Container_libraries); init => AddLibraries(value); }

	/// <remarks>Required Multiple Containment</remarks>
        public bool TryGetLibraries([MaybeNullWhenAttribute(false)] out IReadOnlyList<LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Library> libraries)
	{
		libraries = _libraries;
		return _libraries.Count != 0;
	}

	/// <remarks>Required Multiple Containment</remarks>
    	/// <exception cref = "InvalidValueException">If both Libraries and nodes are empty</exception>
        public Container AddLibraries(IEnumerable<LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Library> nodes)
	{
		var safeNodes = nodes?.ToList();
		AssureNonEmpty(safeNodes, _libraries, MultiLanguage.Instance.Container_libraries);
		_libraries.AddRange(SetSelfParent(safeNodes, MultiLanguage.Instance.Container_libraries));
		return this;
	}

	/// <remarks>Required Multiple Containment</remarks>
    	/// <exception cref = "InvalidValueException">If both Libraries and nodes are empty</exception>
    	/// <exception cref = "ArgumentOutOfRangeException">If index negative or greater than Libraries.Count</exception>
        public Container InsertLibraries(int index, IEnumerable<LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Library> nodes)
	{
		AssureInRange(index, _libraries);
		var safeNodes = nodes?.ToList();
		AssureNonEmpty(safeNodes, _libraries, MultiLanguage.Instance.Container_libraries);
		AssureNoSelfMove(index, safeNodes, _libraries);
		_libraries.InsertRange(index, SetSelfParent(safeNodes, MultiLanguage.Instance.Container_libraries));
		return this;
	}

	/// <remarks>Required Multiple Containment</remarks>
    	/// <exception cref = "InvalidValueException">If Libraries would be empty</exception>
        public Container RemoveLibraries(IEnumerable<LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Library> nodes)
	{
		var safeNodes = nodes?.ToList();
		AssureNotNull(safeNodes, MultiLanguage.Instance.Container_libraries);
		AssureNotClearing(safeNodes, _libraries, MultiLanguage.Instance.Container_libraries);
		RemoveSelfParent(safeNodes, _libraries, MultiLanguage.Instance.Container_libraries);
		return this;
	}

	public Container(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => MultiLanguage.Instance.Container;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (MultiLanguage.Instance.Container_libraries.EqualsIdentity(feature))
		{
			result = Libraries;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (MultiLanguage.Instance.Container_libraries.EqualsIdentity(feature))
		{
			var enumerable = MultiLanguage.Instance.Container_libraries.AsNodes<LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Library>(value).ToList();
			AssureNonEmpty(enumerable, MultiLanguage.Instance.Container_libraries);
			RemoveSelfParent(_libraries.ToList(), _libraries, MultiLanguage.Instance.Container_libraries);
			AddLibraries(enumerable);
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        public override IEnumerable<Feature> CollectAllSetFeatures()
	{
		List<Feature> result = base.CollectAllSetFeatures().ToList();
		if (TryGetLibraries(out _))
			result.Add(MultiLanguage.Instance.Container_libraries);
		return result;
	}

	/// <inheritdoc/>
        protected override bool DetachChild(INode child)
	{
		if (base.DetachChild(child))
			return true;
		Containment? c = GetContainmentOf(child);
		if (MultiLanguage.Instance.Container_libraries.EqualsIdentity(c))
		{
			RemoveSelfParent(child, _libraries, MultiLanguage.Instance.Container_libraries);
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
		if (child is LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Library child0 && _libraries.Contains(child0))
			return MultiLanguage.Instance.Container_libraries;
		return null;
	}
}