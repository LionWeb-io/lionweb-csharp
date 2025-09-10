# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres _loosely_ to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [0.3.0] - tbd

### Added
* Added `JsonUtils.ReadNodesFromStream()` to load nodes from strings synchronously.
* Added benchmarking project.
* Introduced `INotifiableNode`. Represents nodes that support notifications.
* Introduced `INotification` to make a clear distinction between delta protocol-specific events and LionWeb internal event system.
* Introduced `INotificationId`. Represents globally unique id of a notification.
* Added `LionWebVersions` _2025.1_ and _2025.1-compatible_. No content differences to _2024.1_ so far.
* Added `EnumerationLiteralIdentityComparer` and `KeyedIdentityComparer`.
* Added `bool IReadableNode.TryGet(Feature, out object? value)`.
* `JsonUtils.ReadNodesFromStreamAsync()` provides access to the `serializationFormatVersion`.
* Introduced `SerializerBuilder`.
* Introduced `LanguageDeserializerBuilder`.
* Introduced delegating implementations for `SerializerHandler`, `DeserializerHandler`, `LanguageDeserializerHandler`.
* Added async variant `JsonUtils.WriteNodesToStreamAsync()`.
* Introduced `ComparerBehaviorConfig.CompareCompatibleClassifier` to configure whether 
  instances of C# `Annotation` and `Concept` should be considered different.
* Added `M2Extensions.FindByKey()` to search for arbitrary keyed elements in a language.
* Via `ReflectiveBaseNodeFactory.CreateEnumLiteral()` dynamically created C# enums 
  now have proper `LionCoreMetaPointer` attributes.
* Support for model migrations:
  * `IModelMigrator.Migrate()` takes a serialized model, runs all registered `IMigration`s, and serializes the result.
    It works completely with `LenientNode`s and `DynamicLanguage`s (handled by `ModelMigrator`). 
  * Implementations of `IMigration` provide the actual migrations.
  * `MigrationBase` provides a lot of infrastructure to migrate to a new version of a language.
  * `MigrationExtensions` provides static infrastructure methods.
  * `ILanguageRegistry`, available to all `IMigration`s, provides access to languages present during migration.
  * `DynamicLanguageCloner` creates a `DynamicLanguage` clone of any language.
  * We provide two built-in migrations:
    * `LionWebVersionMigration` migrates to a newer version of LionWeb standard.
    * `KeysMigration` migrates changed `IKeyed.Key`s.
  * Have a look at [plugin loading](https://learn.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support)
    to load several versions of the same language in parallel.
* Added `GeneratorConfig` to adjust the supertype of generated interfaces (`INode` vs. `IReadableNode`).
* Added `FeatureClassifierIdentityComparer` distinct from `FeatureIdentityComparer`: The former takes a feature's classifier
  into account, the latter only the hosting language.
* Added `DynamicStructuredDataTypeInstance.TryGet()`
* Added external languages to `DynamicLanguageCloner`.
* Added convenience method `DynamicLanguageClonerExtensions.Clone(Language)`.
### Fixed
* Bug fix: generator now generates correct type for features of builtin Node type
* `LenientNode` now works properly if keys of features change.
* Deserializer can now create instances of languages not registered beforehand.
* Use `[NotNullWhen]` instead of `[MaybeNullWhen]` attributes for _TryGet_-like out parameters.
* Made `M1Extensions.ReplaceWith()` work for annotation instances.
* Enabled replacing `DynamicLanguage.entities`.
* Aligned behavior of `DynamicStructuredDataTypeInstance.CollectAllSetFields()` and `Get()` with other implementations.
* Ensure stable results of `DynamicStructuredDataTypeInstance.Equals()` and `GetHashCode()`.
* `Cloner` can now deal with read-only external reference targets.
* Generator would create uncompilable code if a concept inherits `INamed` via an interface.
* If a node refers to one of its own children, the Serializer would write the reference as containment.
* During deserialization, `ModelMigrator` would add a feature to the wrong language if the node's concept is from a 
  different language as the feature.
* `Comparer` didn't recognize internal references to nodes that are children in single containments.
* `Hasher` would not recognize internal references if it encounters the reference before the target node.
* Deserializing languages with annotations that contain references works now.
### Changed
* Made `DynamicStructuredDataTypeInstance.GetHashCode()` more stable.
* `DynamicLanguageCloner` doesn't change a language's factory anymore.
* `ILanguageRegistry` works on both registered and current round's languages.
* Separated `KeysMigration.languageVersionMapping` into new class `LanguageVersionMigration`.
* Improved naming within `MigrationBase`.
* Simplified `MigrationBase` constructor.
* Clear separation of `MigrationBase` lookup for _origin_ and _destination_.
### Removed
### Deprecated
### Security

## [0.2.4] - 2025-03-13

### Added
* Introduced optionally compressed ids during deserialization.
* Added option to skip serialization of unset features.
* Added Hasher to calculate the hash of a subtree.
* Generate `bool TryGetMyFeature(out MyFeatureType? myFeature)` methods for each feature.
* Added `KeyedDescription` annotation to `ISpecificLanguage` with features `Property documentation: string` and `Reference seeAlso: INode[]`. Generate them to Xdoc.
### Fixed
* Generator flaw when inheriting an interface both directly and through an intermediate classifier
### Changed
* Added explicit LionWebVersions parameter to DynamicIKeyed.
* Separated `Io.Lionweb.Mps.Specific.ISpecificLanguage` into its own project.
### Removed
### Deprecated
### Security

## [0.2.3] - 2025-01-13

### Added
* Introduced `LionWeb.Core.LionWebVersions` to support multiple versions of LionWeb specification in a controlled manner.
  Differences are implemented in specializations of `LionWeb.Core.IVersionSpecifics`:
  * `LionWeb.Core.M1.IDeserializerVersionSpecifics` for Deserializer
  * `LionWeb.Core.IDynamicNodeVersionSpecifics` for DynamicNode
  * `LionWeb.CSharp.Generator.IGeneratorVersionSpecifics` for Generator
* Introduced `LionWeb.Core.M1.IDeserializerHandler` to customize a deserializer's behaviour in non-regular situations.
* Introduced `LionWeb.Core.M1.ISerializerHandler` to customize a serializer's behaviour in non-regular situations.
* Introduced `DeserializerBuilder` to create M1 deserializers.
* Deserializer can resolve references by name / _resolveInfo_ (see `ReferenceResolveInfoHandling`).
* Deserializer supports selecting alternate language versions (see `DeserializerHandlerSelectOtherLanguageVersion`).
* Introduced `Language.LionWebVersion` and `Language.SetFactory()`.
* Introduced `LenientNode` to support migration scenarios.
* Introduced interfaces `IPartitionInstance`, `IAnnotationInstance`, and `IConceptInstance` for applicable nodes.
### Fixed
### Changed
* `LionWeb.Core.Serialization.JsonUtils`:
  * Don't pretty-print serialized JSON anymore.
  * Renamed `ReadNodesFromStreamAsync()` to adhere to C# conventions (added `Async` suffix). 
* Both serializer and deserializer are streaming-enabled, i.e. they can process streams with minimal memory overhead. This changed their API considerably.
* Merged Serializer and LanguageSerializer.
* Generic variants of `Descendants()`, `Children()`, and `Ancestors()` are now public in `M1Extensions`.
* `M2Extensions` methods use immutable collections instead of frozen ones (saves lots of memory).
* `ReflectiveBaseNodeFactory` does not emit warning anymore when creating a node.
* Generator orders all members by name, leading to more stable generation results.
* Generated Factory interfaces, language classes, and classifier types are now `partial`.
* Generated classifier types now _first_ contain all feature members, _then_ `INode` implementations.
### Removed
### Deprecated
* `LionWeb.Core.M2.BuiltInsLanguage`: Use `LionWeb.Core.M2.IBuiltInsLanguage` instead.
* `LionWeb.Core.M3.M3Language`: Use `LionWeb.Core.M3.ILionCoreLanguage` instead.
* `LionWeb.Core.ReleaseVersion`: Use `LionWeb.Core.LionWebVersions` instead.

### Security


## [0.2.2] - 2024-10-21

### Added

- Add following M1 extensions axes methods: `Ancestor`, `PrecedingSiblings`, `FollowingSiblings`.
- Generator adds descriptions to generated classes from model annotations.
- Language (de-)Serialization can handle annotations on languages.
- Generate interface for each factory; factory implementation methods are `virtual` now.
- Add utilities class `ReferenceUtils` (in namespace `LionWeb.Core.Utilities`) to deal with references:
	- `ReferenceValues(<nodes>)` finds all references within all the given `<nodes>`.
	- `FindIncomingReferences(<targetNode(s)>, <nodes>)` finds all references _to_ (all) the `<targetNode(s)>` within the given `<nodes>`.
    - `ReferencesToOutOfScopeNodes(<nodes>)` finds all references to nodes that are not withing the given `<nodes>`.
- Generate `[Obsolete]` attributes on all classes / members with the corresponding annotation `io-lionweb-mps-specific.Deprecated` in LionWeb.
- Generate all types as `partial` to ease customization.

### Fixed

- Fix bug ([issue #7](https://github.com/LionWeb-io/lionweb-csharp/issues/7)) in `Textualizer`: don't crash on unset `name` properties of `INamed`s.

### Changed

- Released as open source under the Apache-2.0 license.
- Set up CI using GitHub Actions.

(Both are infrastructural —i.e.: non-code— changes.)


## [0.2.1] - 2024-05-28

### Fixed

- Fix a small bug w.r.t. `DynamicNode.GetInternal(...)`.
- Update documentation.


## [0.2.0] - 2024-05-27

### Changed

The complete API and structure of the LionWeb C# core NuGet package, as well as the generator and the code it generates, was overhauled.
The generator now resides in its own package.
See [the documents in this directory](./docs) for more information.


## [0.1.10] - 2024-03-19

### Changed

- `Cloner` checks type expectations on features, and throws an exception if they don't hold.


## [0.1.9] - 2024-02-26

## Added

- Deep and detailed node comparer.
- `Annotates` field in the LionCore/M3 `Annotation` type (fixes bug #75989).

### Fixed

- (De-)serialization of `Annotates.Annotation` in language (de-)serializer.

### Changed

- Template-watching mode of code generator works again.
  (Also made various other improvements, such as fixing warnings.)

### Removed

- The `LionWeb-CSharp-Issues` package entirely, since it wasn't used/needed anymore.


## [0.1.8] - 2024-02-06

### Fixed

- Optional properties of type String are serialized correctly.

### Changed

- Required/mandatory (i.e.: non-optional) features generate to C# properties that are non-nullable.


## [0.1.7] - 2024-01-31

### Added

- An out parameter-version of the no-arg factory methods are now also generated.
- Added cloner functionality in the form of the `Cloner` class in the `LionWeb.Utils` namespace.
- Computed properties `AllChildren`, `GetThisAndAllChildren`, `AllDescendants`, `GetThisAndAllDescendants` (=an alias for `AllNodes`) on `Node`, to help with tree navigation.

### Changed

- Use the `INode` type instead of the `Node` type for all generic functionality – except for `SerializationChunk Serializer.Serialize(IEnumerable<Node> nodes)`.
	**Note**: this change _might_ break existing code, and _requires_ to regenerate C# types. 
- Generate nullable properties (and getters/setters) for optional features.


## [0.1.6] - 2024-01-19

### Added

- Added link to sources to published NuGet packages.

### Fixed

- Deserialization now also tries to resolve (references to) annotations in the current model, not just in the dependent models.

### Changed

- Annotations now also show up in the textualization of nodes.

### Removed
### Deprecated
### Security


## [0.1.5] - 2024-01-18

### Added

- With respect to the generated (/generation of) the “nice” factory methods:
	- A `.Create<feature>` method is always generated, even for concepts without features – in which case it's just an alias for the `.New<feature>` method.
	- An overload of the `.Create<feature>` is also generated.
		This overload has an extra `out` argument as _first_ argument.
		With that, you can write statements code like `var y = factory.CreateX(name: "foo", y: factory.CreateY(out var y "bar") );` and be able to use both the `x` and `y` values afterwards.
	- Parameters for the optional, single-valued features are also added to `.Create<feature>` methods – at the end of the method, and with a suitable default value.
		**Breaking change**: the order of parameters might have been changed!
- An extension method `Node.AsString()` that outputs an obvious textual representation of the given `Node`.
	It's useful – e.g. for debugging purposes – to call this from a `.ToString()` method.

### Fixed

- The C# properties that are generated from enumeration-typed properties now have the type of the C# `enum` generated from that enumeration.

### Changed

- With respect to the generated (generation of) the “nice” factory methods:
	- Drop the “To”-part in the names of the .AddTo<Feature>(..) methods.
	- The fluent setters are now used to implement the `.Create<feature>(..)` methods with an expression body.


## [0.1.4] - 2024-01-17

### Added

- Added generation of fluent setters `.Set<Feature>(..)` where the return type is narrowed to the actual type of `this`.
	This means that it's not necessary to downcast to while building trees.
	- Generation templates have been reorganized slightly which causes trivial/irrelevant differences in generated code.
- Added generation of “nice” factory methods that take values for all mandatory, single-valued (so: cardinality=1) features.

### Fixed

- Made the deserializer tolerant towards missing annotations: it now prints a message on the console, and simply ignores the missing annotation.


## [0.1.3] - 2024-01-16

### Added

- Deserialization now takes dependent nodes that can be referenced (as reference targets by ID) from/in the serialization chunk.
- Added generation of fluent `.Set<X>(...)` methods for properties.

### Fixed

- Made LionCore/M3 serialization compliant with specification w.r.t. `annotations` field.

### Changed

- **Breaking change** in the code generated by the generator: generated `.PutInto<X>(...)` methods have been renamed to `.Set<X>(...)` for single-valued features, and to `.AddTo<X>(...)` for multi-valued features (=links).


## [0.1.2] - 2024-01-16

### Added

- This changelog, back-populated to versions tagged as 0.1.1.

### Fixed

- Made the deserializer tolerant towards missing targets of links (=containments and references): it now prints a message on the console, and simply skips the missing link target.


## [0.1.1] - 2024-01-12

### Fixed

- Eagerness to fail in deserializer over missing parent.

### Changed

- Repository structure, and NuGet-related configuration.

