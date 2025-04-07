// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Core.Migration;

using M2;
using M3;
using Utilities;

class DynamicLanguageCloner
{
    private readonly LionWebVersions _lionWebVersion;
    private readonly Dictionary<IKeyed, DynamicIKeyed> _dynamicMap = [];

    public DynamicLanguageCloner(LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
    }

    public Dictionary<LanguageIdentity, DynamicLanguage> Clone(IEnumerable<Language> languages)
    {
        CreateClones(languages);
        ResolveReferences();

        return _dynamicMap
            .Values
            .OfType<DynamicLanguage>()
            .ToDictionary(LanguageIdentity.FromLanguage, l => l);
    }

    #region Cloning

    private void CreateClones(IEnumerable<Language> languages)
    {
        foreach (var l in languages)
        {
            DynamicLanguage dynamicLanguage = CloneLanguage(l);

            foreach (var languageEntity in l.Entities)
            {
                DynamicLanguageEntity entity = languageEntity switch
                {
                    Annotation a => CloneAnnotation(a, dynamicLanguage),
                    Concept c => CloneConcept(c, dynamicLanguage),
                    Interface i => CloneInterface(i, dynamicLanguage),
                    Enumeration e => CloneEnumeration(e, dynamicLanguage),
                    PrimitiveType p => ClonePrimitiveType(p, dynamicLanguage),
                    StructuredDataType s => CloneStructuredDataType(s, dynamicLanguage),
                    _ => throw new ArgumentOutOfRangeException(nameof(languageEntity))
                };

                dynamicLanguage.AddEntities([entity]);
                _dynamicMap.Add(languageEntity, entity);
            }
        }
    }

    private DynamicLanguage CloneLanguage(Language language)
    {
        var result = new DynamicLanguage(language.GetId(), _lionWebVersion)
        {
            Name = language.Name, Key = language.Key, Version = language.Version,
        };
        result.SetFactory(new MigrationFactory(result));
        _dynamicMap.Add(language, result);
        return result;
    }

    private DynamicAnnotation CloneAnnotation(Annotation a, DynamicLanguage language)
    {
        var result = new DynamicAnnotation(a.GetId(), _lionWebVersion, language) { Name = a.Name, Key = a.Key };
        result.AddFeatures(a.Features.Select(CloneFeature));
        return result;
    }

    private DynamicConcept CloneConcept(Concept c, DynamicLanguage language)
    {
        var result = new DynamicConcept(c.GetId(), _lionWebVersion, language)
        {
            Name = c.Name, Key = c.Key, Abstract = c.Abstract, Partition = c.Partition,
        };
        result.AddFeatures(c.Features.Select(CloneFeature));
        return result;
    }

    private DynamicInterface CloneInterface(Interface i, DynamicLanguage language)
    {
        var result = new DynamicInterface(i.GetId(), _lionWebVersion, language) { Name = i.Name, Key = i.Key };
        result.AddFeatures(i.Features.Select(CloneFeature));
        return result;
    }

    private DynamicEnumeration CloneEnumeration(Enumeration enm, DynamicLanguage language)
    {
        var result = new DynamicEnumeration(enm.GetId(), _lionWebVersion, language) { Name = enm.Name, Key = enm.Key };
        result.AddLiterals(enm.Literals.Select(lit =>
        {
            var r = new DynamicEnumerationLiteral(lit.GetId(), _lionWebVersion, result)
            {
                Name = lit.Name, Key = lit.Key
            };
            _dynamicMap.Add(lit, r);
            return r;
        }));
        return result;
    }

    private DynamicLanguageEntity ClonePrimitiveType(PrimitiveType p, DynamicLanguage language) =>
        new DynamicPrimitiveType(p.Key, _lionWebVersion, language) { Name = p.Name, Key = p.Key };

    private DynamicStructuredDataType CloneStructuredDataType(StructuredDataType sdt, DynamicLanguage language)
    {
        var result = new DynamicStructuredDataType(sdt.Key, _lionWebVersion, language)
        {
            Name = sdt.Name, Key = sdt.Key
        };
        result.AddFields(sdt.Fields.Select<Field, DynamicField>(f =>
        {
            var field = new DynamicField(f.GetId(), _lionWebVersion, result) { Name = f.Name, Key = f.Key };
            _dynamicMap.Add(f, field);
            return field;
        }));
        return result;
    }

    private DynamicFeature CloneFeature(Feature f)
    {
        var result = (DynamicFeature)(f switch
        {
            Property p => new DynamicProperty(p.GetId(), _lionWebVersion, null)
            {
                Name = p.Name, Key = p.Key, Optional = p.Optional
            },
            Containment c => new DynamicContainment(c.GetId(), _lionWebVersion, null)
            {
                Name = c.Name, Key = c.Key, Optional = c.Optional, Multiple = c.Multiple
            },
            Reference r => new DynamicReference(r.GetId(), _lionWebVersion, null)
            {
                Name = r.Name, Key = r.Name, Optional = r.Optional, Multiple = r.Multiple
            },
            _ => throw new ArgumentOutOfRangeException(nameof(f))
        });
        _dynamicMap.Add(f, result);
        return result;
    }

    #endregion

    #region References

    private void ResolveReferences()
    {
        foreach (var pair in _dynamicMap)
        {
            switch (pair)
            {
                case (Language i, DynamicLanguage r):
                    r.AddDependsOn(i.DependsOn.Select(Lookup));
                    break;
                case (Annotation i, DynamicAnnotation r):
                    r.Annotates = Lookup(i.Annotates);
                    if (i.TryGetExtends(out var exA))
                        r.Extends = Lookup(exA);
                    r.AddImplements(i.Implements.Select(Lookup));
                    break;
                case (Concept i, DynamicConcept r):
                    if (i.TryGetExtends(out var exC))
                        r.Extends = Lookup(exC);
                    r.AddImplements(i.Implements.Select(Lookup));
                    break;
                case (Interface i, DynamicInterface r):
                    r.AddExtends(i.Extends.Select(Lookup));
                    break;
                case (Property i, DynamicProperty r):
                    r.Type = Lookup(i.Type);
                    break;
                case (Link i, DynamicLink r):
                    r.Type = Lookup(i.Type);
                    break;
                case (Field i, DynamicField r):
                    r.Type = Lookup(i.Type);
                    break;
                case (Datatype, DynamicDatatype):
                case (EnumerationLiteral, DynamicEnumerationLiteral):
                    break;
                default:
                    throw new ArgumentOutOfRangeException(pair.ToString());
            }
        }
    }

    private T Lookup<T>(T keyed) where T : IKeyed?
    {
        if (keyed == null)
        {
            throw new ArgumentException(nameof(T));
        }

        if (keyed.GetLanguage().EqualsIdentity(_lionWebVersion.BuiltIns) ||
            keyed.GetLanguage().Equals(_lionWebVersion.LionCore))
        {
            return keyed;
        }

        if (_dynamicMap.TryGetValue(keyed, out var value))
        {
            if (value is T result)
            {
                return result;
            }
        }

        throw new ArgumentException(nameof(T));
    }

    #endregion
}