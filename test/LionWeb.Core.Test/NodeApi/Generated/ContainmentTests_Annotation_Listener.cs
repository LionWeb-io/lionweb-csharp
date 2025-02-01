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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Languages.Generated.V2024_1.Shapes.M2;
using M1.Event.Partition;

[TestClass]
public class ContainmentTests_Annotation_Listener
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.AddAnnotations([bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { bom });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromOtherParent()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        var oldParent = new Line("oldParent");
        oldParent.AddAnnotations([new Documentation("doc"), bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedFromOtherParentEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.NewParent);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.AddAnnotations([bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromOtherParent_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        var oldParent = new Line("oldParent");
        oldParent.AddAnnotations([new Documentation("doc"), bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedFromOtherParentEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.NewParent);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.Set(null, new List<INode> { bom });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameParent()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([bom, new Documentation("doc")]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedInSameParentEvent>((sender, args) =>
        {
            events++;
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.AddAnnotations([bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameParent_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        var doc = new Documentation("doc");
        line.AddAnnotations([bom, doc]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedInSameParentEvent>((sender, args) =>
        {
            events++;
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.Set(null, new List<INode> { doc, bom });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameParent_NoOp()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([new Documentation("doc"), bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedInSameParentEvent>((sender, args) => events++);

        line.AddAnnotations([bom]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Add_FromSameParent_NoOp_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        var doc = new Documentation("doc");
        line.AddAnnotations([doc, bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedInSameParentEvent>((sender, args) => events++);

        line.Set(null, new List<INode> { doc, bom });

        Assert.AreEqual(0, events);
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(0, [bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(0, [bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_Before_Reflective()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { bom, doc });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(1, [bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After_Reflective()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { doc, bom });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(0, [bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { bom, docA, docB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(1, [bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { docA, bom, docB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(2, [bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { docA, docB, bom });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromOtherParent()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        var oldParent = new Line("oldParent");
        oldParent.AddAnnotations([new Documentation("doc"), bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedFromOtherParentEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.NewParent);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.InsertAnnotations(2, [bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromOtherParent_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        var oldParent = new Line("oldParent");
        oldParent.AddAnnotations([new Documentation("doc"), bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedFromOtherParentEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.NewParent);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.Set(null, new List<INode> { docA, docB, bom });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameParent()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([docA, bom, docB]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedInSameParentEvent>((sender, args) =>
        {
            events++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.InsertAnnotations(2, [bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameParent_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([docA, bom, docB]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedInSameParentEvent>((sender, args) =>
        {
            events++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.Set(null, new List<INode> { docA, docB, bom });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameParent_NoOp()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([docA, bom, docB]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedInSameParentEvent>((sender, args) => events++);

        line.InsertAnnotations(1, [bom]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameParent_NoOp_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([docA, bom, docB]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationMovedInSameParentEvent>((sender, args) => events++);

        line.Set(null, new List<INode> { docA, bom, docB });

        Assert.AreEqual(0, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) => events++);

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var doc = new Documentation("myC");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) => events++);

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Only_Reflective()
    {
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, new List<INode> {  });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom, doc]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_First_Reflective()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom, doc]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, new List<INode> { doc });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last_Reflective()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, bom]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, new List<INode> { doc });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, bom, docB]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, bom, docB]);

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, new List<INode> { docA, docB });

        Assert.AreEqual(1, events);
    }

    #endregion

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) => events++);

        line.AddAnnotations(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) => events++);

        line.Set(null, values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) => events++);

        line.InsertAnnotations(0, values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) => events++);

        line.RemoveAnnotations(values);

        Assert.AreEqual(0, events);
    }


    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([bom]);
        var values = new List<BillOfMaterials>();

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            events++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, values);

        Assert.AreEqual(1, events);
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewAnnotation);
            events++;
        });

        line.AddAnnotations(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewAnnotation);
            events++;
        });

        line.Set(null, values);

        Assert.AreEqual(2, events);
    }

    #region Insert

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewAnnotation);
            events++;
        });

        line.InsertAnnotations(0, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Empty_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewAnnotation);
            events++;
        });

        line.Set(null, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1 + events, args.Index);
            Assert.AreEqual(values[events], args.NewAnnotation);
            events++;
        });

        line.InsertAnnotations(1, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationAddedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1 + events, args.Index);
            Assert.AreEqual(values[events], args.NewAnnotation);
            events++;
        });

        line.Set(null, new List<INode> { docA, valueA, valueB, docB });

        Assert.AreEqual(2, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        line.AddAnnotations([valueA, valueB]);
        var values = new List<INode>() { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_ListMatchingType_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        line.AddAnnotations([valueA, valueB]);
        var values = new List<INode>() { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.Set(null, new List<INode> { });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) => events++);

        line.RemoveAnnotations(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var docA = new Documentation("cA");
        var docB = new Documentation("cB");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) => events++);

        line.RemoveAnnotations(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var docA = new Documentation("cA");
        var docB = new Documentation("cB");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var values = new INode[] { valueA, docA };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(docA, args.DeletedAnnotation);
            events++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained_Reflective()
    {
        var docA = new Documentation("cA");
        var docB = new Documentation("cB");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var values = new INode[] { valueA, docA };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(docA, args.DeletedAnnotation);
            events++;
        });

        line.Set(null, new List<INode> { docB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Only_Reflective()
    {
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.Set(null, new List<INode> {  });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var doc = new Documentation("cId");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Last_Reflective()
    {
        var doc = new Documentation("cId");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.Set(null, new List<INode> { doc });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, valueA, valueB, docB]);
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Between_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, valueA, valueB, docB]);
        var values = new INode[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.Set(null, new List<INode> { docA, docB });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([valueA, docA, valueB, docB]);
        var values = new INode[] { valueA, valueB };

        int events = 0;
        int[] indices = [0, 1];
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(indices[events], args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Mixed_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([valueA, docA, valueB, docB]);
        var values = new INode[] { valueA, valueB };

        int events = 0;
        int[] indices = [0, 1];
        parent.Publisher.Subscribe<AnnotationDeletedEvent>((sender, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(indices[events], args.Index);
            Assert.AreEqual(values[events], args.DeletedAnnotation);
            events++;
        });

        line.Set(null, new List<INode> { docA, docB });

        Assert.AreEqual(2, events);
    }

    #endregion

    #endregion
}