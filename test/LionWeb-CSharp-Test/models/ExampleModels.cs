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

namespace Examples.Shapes.Dynamic;

using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using M2;

public static class ExampleModels
{
    public static INode ExampleLine(Language lang)
    {
        Classifier lineClassifier = lang.ClassifierByKey("key-Line");
        var lineName = lineClassifier.FeatureByKey("LionCore-builtins-INamed-name");
        var lineStart = lineClassifier.FeatureByKey("key-start");
        var lineEnd = lineClassifier.FeatureByKey("key-end");
        var coordClassifier = lang.ClassifierByKey("key-Coord");
        var coordX = coordClassifier.FeatureByKey("key-x");
        var coordY = coordClassifier.FeatureByKey("key-y");
        var coordZ = coordClassifier.FeatureByKey("key-z");

        var line = lang.GetFactory().CreateNode(IdUtils.NewId(), lineClassifier);
        line.Set(lineName, "line1");

        var start = lang.GetFactory().CreateNode(IdUtils.NewId(), coordClassifier);
        start.Set(coordX, -1);
        start.Set(coordY, -3);
        start.Set(coordZ, -3);

        var end = lang.GetFactory().CreateNode(IdUtils.NewId(), coordClassifier);
        end.Set(coordX, 1);
        end.Set(coordY, 2);
        end.Set(coordZ, 3);

        line.Set(lineStart, start);
        line.Set(lineEnd, end);

        return line;
    }

    public static INode ExampleModel(Language lang)
    {
        var language = ShapesLanguage.Instance;
        var geometry = language.GetFactory().CreateNode(IdUtils.NewId(), language.ClassifierByKey("key-Geometry"));
        geometry.Set(language.ClassifierByKey("key-Geometry").FeatureByKey("key-shapes"), new List<INode>{ExampleLine(language)});

        return geometry;
    }
}