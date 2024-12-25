# README

This project contains the .NET implementation of a generator that generates C# source code from a LionWeb M2 defined in LionCore (M3).
The generator uses the Roslyn framework to generate C# ASTs and to unparse those to C# source files.

## C# generator for M2 types

The code generator generates C# code from a language defined using the LionCore M3 types.
Each language generates to a separate file.
Each generated file contains:

* A `class` for each `Annotation` or `Concept` instance, an `interface` for each `Interface` instance, and an `enum` for each `Enumeration` instance.
* Each feature of a `Classifier` generates into a regular field with a getter, a setter, and _no_ default value.
* An implementation of `INodeFactory` to create any concrete (i.e.: non-abstract) `Classifier` from this language.

Remarks:

* The generated `class`es (eventually) extend `NodeBase`.
  The generated `interface`es (eventually) extend `INode`.
* The generated fields maintain the tree structure of the model.
