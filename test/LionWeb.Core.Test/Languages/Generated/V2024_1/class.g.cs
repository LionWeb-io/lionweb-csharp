// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace @namespace.@int.@public;
using LionWeb.Core;
using LionWeb.Core.M1.Event.Partition.Emitter;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2024_1;
using System;
using System.Collections.Generic;
using @base = string;

[LionCoreLanguage(Key = "class", Version = "struct")]
public partial class ClassLanguage : LanguageBase<IClassFactory>
{
	public static readonly ClassLanguage Instance = new Lazy<ClassLanguage>(() => new("id-keyword-lang")).Value;
	public ClassLanguage(string id) : base(id, LionWebVersions.v2024_1)
	{
		_base = new(() => new PrimitiveTypeBase<ClassLanguage>("id-keyword-prim", this) { Key = "key-keyword-prim", Name = "base" });
		_enum = new(() => new EnumerationBase<ClassLanguage>("id-keyword-enm", this) { Key = "key-keyword-enm", Name = "enum", LiteralsLazy = new(() => [enum_internal]) });
		_enum_internal = new(() => new EnumerationLiteralBase<ClassLanguage>("id-keyword-enmA", @enum, this) { Key = "key-keyword-enmA", Name = "internal" });
		_if = new(() => new StructuredDataTypeBase<ClassLanguage>("id-keyword-sdt", this) { Key = "key-keyword-sdt", Name = "if", FieldsLazy = new(() => [if_namespace]) });
		_if_namespace = new(() => new FieldBase<ClassLanguage>("id-keyword-field", @if, this) { Key = "key-keyword-field", Name = "namespace", Type = _builtIns.String });
		_interface = new(() => new InterfaceBase<ClassLanguage>("id-keyword-iface", this) { Key = "key-keyword-iface", Name = "interface", FeaturesLazy = new(() => [interface_string]) });
		_interface_string = new(() => new PropertyBase<ClassLanguage>("id-keyword-prop", @interface, this) { Key = "key-keyword-prop", Name = "string", Optional = false, Type = @enum });
		_out = new(() => new ConceptBase<ClassLanguage>("id-keyword-concept2", this) { Key = "key-keyword-concept2", Name = "out", Abstract = false, Partition = false, ExtendsLazy = new(() => @struct), FeaturesLazy = new(() => [out_default]) });
		_out_default = new(() => new PropertyBase<ClassLanguage>("id-keyword-prop2", @out, this) { Key = "key-keyword-prop2", Name = "default", Optional = false, Type = @if });
		_partial = new(() => new InterfaceBase<ClassLanguage>("id-keyword-iface2", this) { Key = "key-keyword-iface2", Name = "partial", ExtendsLazy = new(() => [@interface]) });
		_record = new(() => new AnnotationBase<ClassLanguage>("id-keyword-ann", this) { Key = "key-keyword-ann", Name = "record", AnnotatesLazy = new(() => _builtIns.Node), ImplementsLazy = new(() => [@interface]), FeaturesLazy = new(() => [record_double]) });
		_record_double = new(() => new ContainmentBase<ClassLanguage>("id-keyword-cont", @record, this) { Key = "key-keyword-cont", Name = "double", Optional = false, Multiple = false, Type = @interface });
		_struct = new(() => new ConceptBase<ClassLanguage>("id-keyword-concept", this) { Key = "key-keyword-concept", Name = "struct", Abstract = false, Partition = false, ImplementsLazy = new(() => [@interface]), FeaturesLazy = new(() => [struct_ref]) });
		_struct_ref = new(() => new ReferenceBase<ClassLanguage>("id-keyword-reference", @struct, this) { Key = "key-keyword-reference", Name = "ref", Optional = false, Multiple = false, Type = @record });
		_var = new(() => new AnnotationBase<ClassLanguage>("id-keyword-ann2", this) { Key = "key-keyword-ann2", Name = "var", AnnotatesLazy = new(() => _builtIns.Node), ExtendsLazy = new(() => @record) });
		_factory = new ClassFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [@enum, @if, @interface, @out, @partial, @record, @struct, @var];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	private const string _key = "class";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "class";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "struct";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<PrimitiveType> _base;
	public PrimitiveType @base => _base.Value;

	private readonly Lazy<Enumeration> _enum;
	public Enumeration @enum => _enum.Value;

	private readonly Lazy<EnumerationLiteral> _enum_internal;
	public EnumerationLiteral enum_internal => _enum_internal.Value;

	private readonly Lazy<StructuredDataType> _if;
	public StructuredDataType @if => _if.Value;

	private readonly Lazy<Field> _if_namespace;
	public Field if_namespace => _if_namespace.Value;

	private readonly Lazy<Interface> _interface;
	public Interface @interface => _interface.Value;

	private readonly Lazy<Property> _interface_string;
	public Property interface_string => _interface_string.Value;

	private readonly Lazy<Concept> _out;
	public Concept @out => _out.Value;

	private readonly Lazy<Property> _out_default;
	public Property out_default => _out_default.Value;

	private readonly Lazy<Interface> _partial;
	public Interface @partial => _partial.Value;

	private readonly Lazy<Annotation> _record;
	public Annotation @record => _record.Value;

	private readonly Lazy<Containment> _record_double;
	public Containment record_double => _record_double.Value;

	private readonly Lazy<Concept> _struct;
	public Concept @struct => _struct.Value;

	private readonly Lazy<Reference> _struct_ref;
	public Reference struct_ref => _struct_ref.Value;

	private readonly Lazy<Annotation> _var;
	public Annotation @var => _var.Value;
}

public partial interface IClassFactory : INodeFactory
{
	public @out Newout(string id);
	public @out Createout();
	public @record Newrecord(string id);
	public @record Createrecord();
	public @struct Newstruct(string id);
	public @struct Createstruct();
	public @var Newvar(string id);
	public @var Createvar();
}

public class ClassFactory : AbstractBaseNodeFactory, IClassFactory
{
	private readonly ClassLanguage _language;
	public ClassFactory(ClassLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.@out.EqualsIdentity(classifier))
			return Newout(id);
		if (_language.@record.EqualsIdentity(classifier))
			return Newrecord(id);
		if (_language.@struct.EqualsIdentity(classifier))
			return Newstruct(id);
		if (_language.@var.EqualsIdentity(classifier))
			return Newvar(id);
		throw new UnsupportedClassifierException(classifier);
	}

	/// <inheritdoc/>
        public override Enum GetEnumerationLiteral(EnumerationLiteral literal)
	{
		if (_language.@enum.EqualsIdentity(literal.GetEnumeration()))
			return EnumValueFor<@enum>(literal);
		throw new UnsupportedEnumerationLiteralException(literal);
	}

	/// <inheritdoc/>
        public override IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType, IFieldValues fieldValues)
	{
		if (_language.@if.EqualsIdentity(structuredDataType))
			return new @if((string?)fieldValues.Get(_language.if_namespace));
		throw new UnsupportedStructuredDataTypeException(structuredDataType);
	}

	public virtual @out Newout(string id) => new(id);
	public virtual @out Createout() => Newout(GetNewId());
	public virtual @record Newrecord(string id) => new(id);
	public virtual @record Createrecord() => Newrecord(GetNewId());
	public virtual @struct Newstruct(string id) => new(id);
	public virtual @struct Createstruct() => Newstruct(GetNewId());
	public virtual @var Newvar(string id) => new(id);
	public virtual @var Createvar() => Newvar(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-iface")]
public partial interface @interface : INode
{
	/// <remarks>Required Property</remarks>
        [LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-prop")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public @enum String { get; set; }

	/// <remarks>Required Property</remarks>
        public @interface SetString(@enum value);
}

[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-concept2")]
public partial class @out : @struct
{
	private @if? _default = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Default has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-prop2")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public @if Default { get => _default ?? throw new UnsetFeatureException(ClassLanguage.Instance.out_default); set => SetDefault(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public @out SetDefault(@if value)
	{
		AssureNotNull(value, ClassLanguage.Instance.out_default);
		PropertyEventEmitter evt = new(ClassLanguage.Instance.out_default, this, value, _default);
		evt.CollectOldData();
		_default = value;
		evt.RaiseEvent();
		return this;
	}

	public @out(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => ClassLanguage.Instance.@out;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (ClassLanguage.Instance.out_default.EqualsIdentity(feature))
		{
			result = Default;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (ClassLanguage.Instance.out_default.EqualsIdentity(feature))
		{
			if (value is @namespace.@int.@public.@if v)
			{
				Default = v;
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
		if (_default != default)
			result.Add(ClassLanguage.Instance.out_default);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-iface2")]
public partial interface @partial : @interface
{
}

[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-ann")]
public partial class @record : AnnotationInstanceBase, @interface
{
	private @enum? _string = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If String has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-prop")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public @enum String { get => _string ?? throw new UnsetFeatureException(ClassLanguage.Instance.interface_string); set => SetString(value); }
/// <remarks>Required Property</remarks>
/// <exception cref="InvalidValueException">If set to null</exception>
 @interface @interface.SetString(@enum value) => SetString(value);
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public @record SetString(@enum value)
	{
		AssureNotNull(value, ClassLanguage.Instance.interface_string);
		PropertyEventEmitter evt = new(ClassLanguage.Instance.interface_string, this, value, _string);
		evt.CollectOldData();
		_string = value;
		evt.RaiseEvent();
		return this;
	}

	private @interface? _double = null;
	/// <remarks>Required Single Containment</remarks>
    	/// <exception cref = "UnsetFeatureException">If Double has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-cont")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Containment, Optional = false, Multiple = false)]
	public @interface Double { get => _double ?? throw new UnsetFeatureException(ClassLanguage.Instance.record_double); set => SetDouble(value); }

	/// <remarks>Required Single Containment</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public @record SetDouble(@interface value)
	{
		AssureNotNull(value, ClassLanguage.Instance.record_double);
		ContainmentSingleEventEmitter<@interface> evt = new(ClassLanguage.Instance.record_double, this, value, _double);
		evt.CollectOldData();
		SetParentNull(_double);
		AttachChild(value);
		_double = value;
		evt.RaiseEvent();
		return this;
	}

	public @record(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Annotation GetAnnotation() => ClassLanguage.Instance.@record;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (ClassLanguage.Instance.interface_string.EqualsIdentity(feature))
		{
			result = String;
			return true;
		}

		if (ClassLanguage.Instance.record_double.EqualsIdentity(feature))
		{
			result = Double;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (ClassLanguage.Instance.interface_string.EqualsIdentity(feature))
		{
			if (value is @namespace.@int.@public.@enum v)
			{
				String = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (ClassLanguage.Instance.record_double.EqualsIdentity(feature))
		{
			if (value is @namespace.@int.@public.@interface v)
			{
				Double = v;
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
		if (_string != default)
			result.Add(ClassLanguage.Instance.interface_string);
		if (_double != default)
			result.Add(ClassLanguage.Instance.record_double);
		return result;
	}

	/// <inheritdoc/>
        protected override bool DetachChild(INode child)
	{
		if (base.DetachChild(child))
			return true;
		Containment? c = GetContainmentOf(child);
		if (ClassLanguage.Instance.record_double.EqualsIdentity(c))
		{
			_double = null;
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
		if (ReferenceEquals(_double, child))
			return ClassLanguage.Instance.record_double;
		return null;
	}
}

[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-concept")]
public partial class @struct : ConceptInstanceBase, @interface
{
	private @enum? _string = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If String has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-prop")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public @enum String { get => _string ?? throw new UnsetFeatureException(ClassLanguage.Instance.interface_string); set => SetString(value); }
/// <remarks>Required Property</remarks>
/// <exception cref="InvalidValueException">If set to null</exception>
 @interface @interface.SetString(@enum value) => SetString(value);
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public @struct SetString(@enum value)
	{
		AssureNotNull(value, ClassLanguage.Instance.interface_string);
		PropertyEventEmitter evt = new(ClassLanguage.Instance.interface_string, this, value, _string);
		evt.CollectOldData();
		_string = value;
		evt.RaiseEvent();
		return this;
	}

	private @record? _ref = null;
	/// <remarks>Required Single Reference</remarks>
    	/// <exception cref = "UnsetFeatureException">If Ref has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-reference")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Reference, Optional = false, Multiple = false)]
	public @record Ref { get => _ref ?? throw new UnsetFeatureException(ClassLanguage.Instance.struct_ref); set => SetRef(value); }

	/// <remarks>Required Single Reference</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public @struct SetRef(@record value)
	{
		AssureNotNull(value, ClassLanguage.Instance.struct_ref);
		ReferenceSingleEventEmitter evt = new(ClassLanguage.Instance.struct_ref, this, value, _ref);
		evt.CollectOldData();
		_ref = value;
		evt.RaiseEvent();
		return this;
	}

	public @struct(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => ClassLanguage.Instance.@struct;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (ClassLanguage.Instance.interface_string.EqualsIdentity(feature))
		{
			result = String;
			return true;
		}

		if (ClassLanguage.Instance.struct_ref.EqualsIdentity(feature))
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
		if (ClassLanguage.Instance.interface_string.EqualsIdentity(feature))
		{
			if (value is @namespace.@int.@public.@enum v)
			{
				String = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (ClassLanguage.Instance.struct_ref.EqualsIdentity(feature))
		{
			if (value is @namespace.@int.@public.@record v)
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
		if (_string != default)
			result.Add(ClassLanguage.Instance.interface_string);
		if (_ref != default)
			result.Add(ClassLanguage.Instance.struct_ref);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-ann2")]
public partial class @var : @record
{
	public @var(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Annotation GetAnnotation() => ClassLanguage.Instance.@var;
}

[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-enm")]
public enum @enum
{
	[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-enmA")]
	@internal
}

[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-sdt")]
public readonly record struct @if : IStructuredDataTypeInstance
{
	private readonly string? _namespace;
	[LionCoreMetaPointer(Language = typeof(ClassLanguage), Key = "key-keyword-field")]
	public string Namespace { get => _namespace ?? throw new UnsetFieldException(ClassLanguage.Instance.if_namespace); init => _namespace = value; }

	public @if()
	{
		_namespace = null;
	}

	internal @if(string? @namespace)
	{
		_namespace = @namespace;
	}

	/// <inheritdoc/>
        public StructuredDataType GetStructuredDataType() => ClassLanguage.Instance.@if;
	/// <inheritdoc/>
        public IEnumerable<Field> CollectAllSetFields()
	{
		List<Field> result = [];
		if (_namespace != null)
			result.Add(ClassLanguage.Instance.if_namespace);
		return result;
	}

	/// <inheritdoc/>
        public Object? Get(Field field)
	{
		if (ClassLanguage.Instance.if_namespace.EqualsIdentity(field))
			return Namespace;
		throw new UnsetFieldException(field);
	}

	public override string ToString() => $"@if {{ Namespace = {_namespace} }}";
}