# README

This project contains the C# implementation of (parts of) the [LionWeb specification](https://github.com/lionWeb-io/specification/).
Specifically supported are:

* language definition through the [LionCore M3 meta-metamodel](https://github.com/LionWeb-io/specification/blob/main/metametamodel/metametamodel.adoc),
* serialization and deserialization of both languages and instances of those conforming to the [LionWeb JSON serialization format](https://github.com/LionWeb-io/specification/blob/main/serialization/serialization.adoc),
* a generic, dynamic/reflective base implementation of nodes,
* C# type generation.

For convenience, the UML representation of the LionCore M3 meta-metamodel, including the separate LionCore built-ins language, is reproduced here:

![A UML representation of the LionCore M3 meta-metamodel and LionCore built-ins](docs/metametamodel-with-complete-builtins.png)

The LionCore built-ins language contains several classifiers (=instances of `Classifier`, so reside on the M2-level because they don't admit a self-definition) that can be used within any language definition.
These specifically are

* the `INamed` interface – because many concepts have a "name" property,
* the generic `Node` concept – to use as the type of the target(s) of an untyped reference relation,
* the four built-in primitive types.


## Project organization

Various aspects of LionWeb are represented as directories at the top-level of this project.

* `core/`:
    * [base types](core/BaseTypes.cs), [`Node`, a generic, dynamic/reflective implementation of `INode`](core/Node.cs), [release version declaration](core/ReleaseVersion.cs)
    * `M2/`: [LionCore built-ins](core/M2/BuiltIns.cs)
    * `M3/`: [implementation of the LionCore M3](core/M3/Types.cs) to define languages with, computed properties and extension methods defined on top of those, and (de-)serialization of language definitions
	* `serialization/`: implementation of (de-)serialization for the `Node` type, and definition of the serialization chunk format (after it's been unmarshalled from JSON)
* `docs/`: documentation, mainly in the form of diagrams
* `utils/`: utilities, mainly intended for internal use but occasionally – such as `JsonUtils` – useful outside of that as well
* `generator/`: a C# generator for M2 types - see the [`M2TypesGenerator` class](generator/M2TypesGenerator.cs) and [`MultiLanguageNodeFactoryGenerator`](generator/MultiLanguageNodeFactoryGenerator.cs) classes


## C# generator for M2 types

The [`M2TypesGenerator` class](generator/M2TypesGenerator.cs) code generator generates C# code from a language defined using the [LionCore M3 types](core/M3/Types.cs).
Each language generates to a separate file.
Each generated file contains:

* A `class` for each `Concept` instance, an `interface` for each `Interface` instance, and an `enum` for each `Enumeration` instance.
* Each feature of a `Classifier` (i.e., a `Concept` or `Interface` – note that `Annotation` isn't supported yet) generates into a regular field with a getter, a setter, and _no_ default value.
* An implementation of `INodeFactory` to create any concrete (i.e.: non-abstract) `Concept` from this language, with a placeholder instance of `Node` for unknown `Concept`s.

Remarks:

* The generated `class`es (eventually) extend `Node`.
    The generated `interface`es (eventually) extend `INode`.
* The generated fields are quite "dumb", and provide no convenience – at least, not at the moment.
    Care must be taken to "do the right thing".
    In particular, setting containment is done by setting the `_Parent` field of a node _and_ adding the node to an appropriate field of that parent.


### Generator details

The generators (for M2 types and _"multi-factory"_) receive a `TemplatesManager` which discovers, loads, and compiles the templates in the `generator/templates/` directory.
A generator then invokes the `GeneratorInstantiator<T>(string mainTemplateName)` method to instantiate a `CodeGenerator` function which runs the indicated Handlebars main template with input data of the indicated type `T`.
By invoking the `WatchTemplates` method of a `TemplatesManager` instance, a _"watch mode"_ is triggered.
Anytime a change occurs to the templates directory, a given action (typically: invoking a `CodeGenerator` function and saving the resulting string to file) is rerun.
This is useful for working on the Handlebars templates without needing to restart entirely.

The C# code of a generator can be watched by executing a command of the following form:

```shell
$ dotnet watch run <main program>.cs
```


## API

### Languages

Serializing instances of `Language` as a [LionWeb serialization](https://github.com/LionWeb-io/specification/blob/main/serialization/serialization.adoc) chunk can be done as follows:

```csharp
// serialization to internal format:
using LionWeb.Core.M2;
SerializationChunk serializationChunk = LanguageSerializer.Serialize(languages);

// serialization of internal format to JSON:
using LionWeb.Utils;
JsonUtils.WriteJsonToFile(<path>, serializationChunk);
```

(Also note that code in these snippets – in particular the `using` statements – might not be syntactically correct.
 Adjust before use.)

Deserializing a LionWeb serialization chunk containing one or more languages can be done as follows:

```csharp
// read the JSON:
using LionWeb.Utils;
var serializationChunk = JsonUtils.ReadJsonFromFile<SerializationChunk>(<path>);

// perform the deserialization:
using LionWeb.Core.M3;
IEnumerable<Language> languages = LanguageDeserializer.Deserialize(serializationChunk);
```


### Instances

Serializing nodes (as instances of `Node`) can be done as follows:

```csharp
// serialization to internal format:
using LionWeb.Core.Serialization;
var serializationChunk = Serializer.Serialize(<nodes>);

// serialization of internal format to JSON:
using LionWeb.Utils;
JsonUtils.WriteJsonToFile(<path>, serializationChunk);
```

Deserializing a LionWeb serialization chunk that's the serialization of nodes from one language can be done as follows:

```csharp
// read the JSON:
using LionWeb.Utils;
var serializationChunk = JsonUtils.ReadJsonFromFile<SerializationChunk>(<path>);

// perform the deserialization:
using LionWeb.Core;
using LionWeb.Core.M3;
var (Language language, INodeFactory factory) languageWithFactory = (<language instance>, <its corresponding INodeFactory implementation>);
var deserializer = new Deserializer(languageWithFactory);
using LionWeb.Core.Serialization;
List<Node> nodes = deserializer.Deserialize(serializationChunk);
```

The argument to the constructor of the `Deserializer` is a [parameter array](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/method-parameters?redirectedfrom=MSDN#params-modifier), so add a tuple with a `Language` instance and a corresponding implementation of `INodeFactory` for all languages that the serialization chunk uses – see its `languages` field.

