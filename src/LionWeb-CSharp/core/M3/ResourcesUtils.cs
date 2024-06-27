// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.M3;

using System.Reflection;

/// <summary>
/// Utility methods for working with .NET resources.
/// </summary>
public static class ResourcesUtils
{
    /// <summary>
    /// Obtains and <returns/> the assembly with the given <paramref name="assemblyName">name</paramref>.
    /// </summary>
    public static Assembly GetAssemblyByName(string assemblyName)
        => AppDomain.CurrentDomain.GetAssemblies()
            .Single(assembly => assembly.GetName().Name == assemblyName);
    // TODO  Q: does a better/easier way exist (without having to mention the assembly's name)?

    /// <summary>
    /// Obtains and <returns/> the resource with the given <paramref name="resourceName">name</paramref> from the given <paramref name="assembly">assembly</paramref>.
    /// </summary>
    public static string GetResourceAsText(Assembly assembly, string resourceName)
    {
        using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Obtains and <returns/> a dictionary (file name without given <paramref name="extension"/> &#8594; contents as text)
    /// containing all files in the given <paramref name="assembly"/> under the given <paramref name="dirPath">path</paramref>
    /// with the given <paramref name="extension">file extension</paramref>.
    /// </summary>
    public static IDictionary<string, string> AllResourcesByFileName(Assembly assembly, string dirPath,
        string extension)
    {
        var templatesMap = new Dictionary<string, string>();

        var prefix = $"{assembly.GetName().Name}.{dirPath.Replace('/', '.')}";
        // TODO  use this to implement an alternative to GetResourceAsText that builds up the resource name from a path
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(prefix) && name.EndsWith($".{extension}"));
        foreach (var resourceName in resourceNames)
        {
            var key = resourceName.Substring(
                prefix.Length,
                resourceName.Length - (1 + prefix.Length + extension.Length)
            );
            templatesMap[key] = GetResourceAsText(assembly, resourceName);
        }

        return templatesMap;
    }
}