// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Notification;

using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;

[TestClass]
public class UnresolvedReferencesManagerTest : NotificationTestsBase
{
    private static readonly Reference _reference_0_1 = TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1;
    private static readonly Reference _reference_0_n = TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n;

    [TestMethod]
    public void AddChildWithUnresolvedReference_Single()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        var referenceTarget = new ReferenceTarget(null, "target", null);
        parent.Set(_reference_0_1, referenceTarget);
        partition.AddLinks([parent]);

        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._registered);

        var target = new LinkTestConcept("target");
        partition.AddLinks([target]);
        
        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._resolved);
        Assert.AreSame(target, parent.Reference_0_1);
    }
    
    [TestMethod]
    public void AddChildWithUnresolvedReference_Multiple()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        var referenceTargetA = new ReferenceTarget("A", "target", null);
        var referenceTargetB = new ReferenceTarget("B", "target", null);
        parent.Set(_reference_0_n, new List<IReferenceTarget>{referenceTargetA, referenceTargetB});
        partition.AddLinks([parent]);

        Assert.Contains((parent, _reference_0_n, referenceTargetA), manager._registered);
        Assert.Contains((parent, _reference_0_n, referenceTargetB), manager._registered);

        var target = new LinkTestConcept("target");
        partition.AddLinks([target]);
        
        Assert.Contains((parent, _reference_0_n, referenceTargetA), manager._resolved);
        Assert.Contains((parent, _reference_0_n, referenceTargetB), manager._resolved);
        Assert.AreSame(target, parent.Reference_0_n[0]);
        Assert.AreSame(target, parent.Reference_0_n[1]);
    }
    
    [TestMethod]
    public void RemoveChildWithUnresolvedReference_Single()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        var referenceTarget = new ReferenceTarget(null, "target", null);
        parent.Set(_reference_0_1, referenceTarget);
        partition.AddLinks([parent]);

        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._registered);

        parent.DetachFromParent();
        
        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._unregistered);
    }
    
    [TestMethod]
    public void RemoveChildWithUnresolvedReference_Multiple()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        var referenceTargetA = new ReferenceTarget("A", "target", null);
        var referenceTargetB = new ReferenceTarget("B", "target", null);
        parent.Set(_reference_0_n, new List<IReferenceTarget>{referenceTargetA, referenceTargetB});
        partition.AddLinks([parent]);

        Assert.Contains((parent, _reference_0_n, referenceTargetA), manager._registered);
        Assert.Contains((parent, _reference_0_n, referenceTargetB), manager._registered);

        parent.DetachFromParent();
        
        Assert.Contains((parent, _reference_0_n, referenceTargetA), manager._unregistered);
        Assert.Contains((parent, _reference_0_n, referenceTargetB), manager._unregistered);
    }
    
    [TestMethod]
    public void RemoveDescendantWithUnresolvedReference_Single()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);

        var grandParent = new LinkTestConcept("grandparent");
        
        var parent = new LinkTestConcept("parent");
        var referenceTarget = new ReferenceTarget(null, "target", null);
        parent.Set(_reference_0_1, referenceTarget);

        grandParent.Containment_0_1 = parent;
        partition.AddLinks([grandParent]);

        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._registered);

        grandParent.DetachFromParent();
        
        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._unregistered);
    }

    [TestMethod]
    public void ReplaceDescendantReferences()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);

        var grandParent = new LinkTestConcept("grandparent");
        var targetInsideReplaced = new ReferenceTarget(null, "insideReplaced", null);
        grandParent.Set(_reference_0_1, targetInsideReplaced);

        partition.AddLinks([grandParent]);

        Assert.Contains((grandParent, _reference_0_1, targetInsideReplaced), manager._registered);

        var oldParent = new LinkTestConcept("oldParent");
        var oldTarget = new ReferenceTarget(null, "oldTarget", null);
        oldParent.Set(_reference_0_1, oldTarget);
        grandParent.Containment_1 = oldParent;

        Assert.Contains((oldParent, _reference_0_1, oldTarget), manager._registered);

        var newParent = new LinkTestConcept("newParent");
        var targetNonExistent = new ReferenceTarget(null, "nonExistent", null);
        newParent.Set(_reference_0_1, targetNonExistent);
        var insideReplaced = new LinkTestConcept("insideReplaced");
        newParent.Containment_0_1 = insideReplaced;

        oldParent.ReplaceWith(newParent);

        Assert.Contains((oldParent, _reference_0_1, oldTarget), manager._unregistered);
        
        Assert.Contains((newParent, _reference_0_1, targetNonExistent), manager._registered);
        
        Assert.Contains((grandParent, _reference_0_1, targetInsideReplaced), manager._resolved);
        Assert.AreSame(insideReplaced, grandParent.Reference_0_1);
    }

    [TestMethod]
    public void AddUnresolvedReference_Single()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        partition.AddLinks([parent]);
        
        var referenceTarget = new ReferenceTarget(null, "target", null);
        parent.Set(_reference_0_1, referenceTarget);

        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._registered);

        var target = new LinkTestConcept("target");
        partition.AddLinks([target]);
        
        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._resolved);
        Assert.AreSame(target, parent.Reference_0_1);
    }
    
    [TestMethod]
    public void AddUnresolvedReference_Multiple()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        partition.AddLinks([parent]);
        
        var referenceTargetA = new ReferenceTarget("A", "target", null);
        var referenceTargetB = new ReferenceTarget("B", "target", null);
        parent.Set(_reference_0_n, new List<IReferenceTarget>{referenceTargetA, referenceTargetB});

        Assert.Contains((parent, _reference_0_n, referenceTargetA), manager._registered);
        Assert.Contains((parent, _reference_0_n, referenceTargetB), manager._registered);

        var target = new LinkTestConcept("target");
        partition.AddLinks([target]);
        
        Assert.Contains((parent, _reference_0_n, referenceTargetA), manager._resolved);
        Assert.Contains((parent, _reference_0_n, referenceTargetB), manager._resolved);
        Assert.AreSame(target, parent.Reference_0_n[0]);
        Assert.AreSame(target, parent.Reference_0_n[1]);
    }
    
    [TestMethod]
    public void DeleteUnresolvedReference_Single()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        partition.AddLinks([parent]);
        
        var referenceTarget = new ReferenceTarget(null, "target", null);
        parent.Set(_reference_0_1, referenceTarget);

        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._registered);

        parent.Reference_0_1 = null;
        
        Assert.Contains((parent, _reference_0_1, referenceTarget), manager._unregistered);
    }
    
    [TestMethod]
    public void DeleteUnresolvedReference_Multiple()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        partition.AddLinks([parent]);
        
        var referenceTargetA = new ReferenceTarget("A", "target", null);
        var referenceTargetB = new ReferenceTarget("B", "target", null);
        parent.Set(_reference_0_n, new List<IReferenceTarget>{referenceTargetA, referenceTargetB});

        Assert.Contains((parent, _reference_0_n, referenceTargetA), manager._registered);
        Assert.Contains((parent, _reference_0_n, referenceTargetB), manager._registered);

        parent.Set(_reference_0_n, new List<IReferenceTarget>());
        
        Assert.Contains((parent, _reference_0_n, referenceTargetA), manager._unregistered);
        Assert.Contains((parent, _reference_0_n, referenceTargetB), manager._unregistered);
    }
    
    [TestMethod]
    public void ChangeUnresolvedReference_Single()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        partition.AddLinks([parent]);
        
        var oldTarget = new ReferenceTarget(null, "oldTarget", null);
        parent.Set(_reference_0_1, oldTarget);
        Assert.Contains((parent, _reference_0_1, oldTarget), manager._registered);
        
        var newTarget = new ReferenceTarget(null, "newTarget", null);
        parent.Set(_reference_0_1, newTarget);
        Assert.Contains((parent, _reference_0_1, oldTarget), manager._unregistered);
        Assert.Contains((parent, _reference_0_1, newTarget), manager._registered);

        var target = new LinkTestConcept("newTarget");
        partition.AddLinks([target]);
        
        Assert.DoesNotContain((parent, _reference_0_1, oldTarget), manager._resolved);
        Assert.Contains((parent, _reference_0_1, newTarget), manager._resolved);
        Assert.AreSame(target, parent.Reference_0_1);
    }
    
    [TestMethod]
    public void ChangeUnresolvedReference_Multiple()
    {
        var partition = new TestPartition("partition");
        var manager = new TestUnresolvedReferenceManager(true);
        partition.GetNotificationSender()!.ConnectTo(manager);
        
        var parent = new LinkTestConcept("parent");
        partition.AddLinks([parent]);
        
        var oldTargetA = new ReferenceTarget("A", "oldTarget", null);
        var oldTargetB = new ReferenceTarget("B", "newTarget", null);
        parent.Set(_reference_0_n, new List<IReferenceTarget>{oldTargetA, oldTargetB});
        Assert.Contains((parent, _reference_0_n, oldTargetA), manager._registered);
        Assert.Contains((parent, _reference_0_n, oldTargetB), manager._registered);
        
        var newTarget = new ReferenceTarget(null, "newTarget", null);
        parent.Set(_reference_0_n, new List<IReferenceTarget>{newTarget, oldTargetB});
        Assert.Contains((parent, _reference_0_n, oldTargetA), manager._unregistered);
        Assert.DoesNotContain((parent, _reference_0_n, oldTargetB), manager._unregistered);
        Assert.Contains((parent, _reference_0_n, newTarget), manager._registered);

        var target = new LinkTestConcept("newTarget");
        partition.AddLinks([target]);
        
        Assert.DoesNotContain((parent, _reference_0_n, oldTargetA), manager._resolved);
        Assert.Contains((parent, _reference_0_n, newTarget), manager._resolved);
        Assert.Contains((parent, _reference_0_n, oldTargetB), manager._resolved);
        Assert.AreSame(target, parent.Reference_0_n[0]);
        Assert.AreSame(target, parent.Reference_0_n[1]);
    }
    
}

internal class TestUnresolvedReferenceManager(bool registerAddedUnresolvedReferences = false) : UnresolvedReferencesManager(registerAddedUnresolvedReferences)
{
    internal readonly List<(IWritableNode parent, Feature reference, IReferenceTarget target)> _registered = [];
    internal readonly List<(IWritableNode parent, Feature reference, IReferenceTarget target)> _unregistered = [];
    internal readonly List<(IWritableNode parent, Feature reference, IReferenceTarget target)> _resolved = [];

    protected override void LogRegister(IWritableNode parent, Feature reference, IReferenceTarget target)
    {
        base.LogRegister(parent, reference, target);
        _registered.Add((parent, reference, target));
    }

    protected override void LogUnregister(IWritableNode parent, Feature reference, IReferenceTarget target)
    {
        base.LogUnregister(parent, reference, target);
        _unregistered.Add((parent, reference, target));
    }

    protected override void LogResolve(IWritableNode parent, Feature reference, IReferenceTarget target)
    {
        base.LogResolve(parent, reference, target);
        _resolved.Add((parent, reference, target));
    }
}