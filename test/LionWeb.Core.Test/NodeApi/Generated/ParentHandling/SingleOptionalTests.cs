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

namespace LionWeb.Core.Test.NodeApi.Generated.ParentHandling;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SingleOptionalTests
{
    #region SameInOtherInstance

    [TestMethod]
    public void SameInOtherInstance()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new Geometry("tgt");

        target.Documentation = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(source.Documentation);
    }

    [TestMethod]
    public void SameInOtherInstance_detach()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var orphan = new Documentation("o");
        var target = new Geometry("tgt") { Documentation = orphan };

        target.Documentation = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(source.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SameInOtherInstance_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(source.Documentation);
    }

    [TestMethod]
    public void SameInOtherInstance_detach_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var orphan = new Documentation("o");
        var target = new Geometry("tgt") { Documentation = orphan };

        target.Set(ShapesLanguage.Instance.Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(source.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region other

    [TestMethod]
    public void Other()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new OffsetDuplicate("tgt");

        target.Docs = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Docs);
        Assert.IsNull(source.Documentation);
    }

    [TestMethod]
    public void Other_detach()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var orphan = new Documentation("o");
        var target = new OffsetDuplicate("tgt") { Docs = orphan };

        target.Docs = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Docs);
        Assert.IsNull(source.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new OffsetDuplicate("tgt");

        target.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Docs);
        Assert.IsNull(source.Documentation);
    }

    [TestMethod]
    public void Other_detach_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var orphan = new Documentation("o");
        var target = new OffsetDuplicate("tgt") { Docs = orphan };

        target.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Docs);
        Assert.IsNull(source.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region otherInSameInstance

    [TestMethod]
    public void OtherInSameInstance()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.SecretDocs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void OtherInSameInstance_detach()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var parent = new OffsetDuplicate("src") { Docs = child, SecretDocs = orphan };

        parent.SecretDocs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_secretDocs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void OtherInSameInstance_detach_Reflective()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var parent = new OffsetDuplicate("src") { Docs = child, SecretDocs = orphan };

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_secretDocs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region sameInSameInstance

    [TestMethod]
    public void SameInSameInstance()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.SecretDocs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void SameInSameInstance_Reflective()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Docs);
    }

    #endregion

    #region annotation

    [TestMethod]
    public void ToAnnotation()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new Line("tgt");

        target.AddAnnotations([child]);

        Assert.AreSame(target, child.GetParent());
        Assert.IsTrue(target.GetAnnotations().Contains(child));
        Assert.IsFalse(source.GetAnnotations().Contains(child));
    }

    [TestMethod]
    public void ToAnnotation_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new Line("tgt");

        target.Set(null, new List<INode> { child });

        Assert.AreSame(target, child.GetParent());
        Assert.IsTrue(target.GetAnnotations().Contains(child));
        Assert.IsFalse(source.GetAnnotations().Contains(child));
    }

    [TestMethod]
    public void Annotation_ToSingleOptional()
    {
        var child = new Documentation("myId");
        var source = new Line("src");
        source.AddAnnotations([child]);
        var target = new Geometry("tgt");

        target.Documentation = child;

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Documentation);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_detach()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var source = new Line("src");
        source.AddAnnotations([child]);
        var target = new Geometry("tgt") { Documentation = orphan };

        target.Documentation = child;

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Line("src");
        source.AddAnnotations([child]);
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Documentation);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_detach_Reflective()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var source = new Line("src");
        source.AddAnnotations([child]);
        var target = new Geometry("tgt") { Documentation = orphan };

        target.Set(ShapesLanguage.Instance.Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    #region sameInstance

    [TestMethod]
    public void ToAnnotation_SameInstance()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.AddAnnotations([child]);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(child));
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void ToAnnotation_SameInstance_Reflective()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.Set(null, new List<INode> { child });

        Assert.AreSame(parent, child.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(child));
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src");
        parent.AddAnnotations([child]);

        parent.Docs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Docs);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance_detach()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var parent = new OffsetDuplicate("src") { Docs = orphan };
        parent.AddAnnotations([child]);

        parent.Docs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Docs);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance_Reflective()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src");
        parent.AddAnnotations([child]);

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Docs);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance_detach_Reflective()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var parent = new OffsetDuplicate("src") { Docs = orphan };
        parent.AddAnnotations([child]);

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Docs);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #endregion
}