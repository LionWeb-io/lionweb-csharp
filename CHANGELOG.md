# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres _loosely_ to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
**TODO**  make the “loosely” more precise


## [Unreleased]

### Added
### Fixed
### Changed
### Removed
### Deprecated
### Security

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

