// <copyright file="SetterCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.PresentationCore.Interactivity
{
  public sealed class SetterCollection : SetterCollectionBase
  {
    #region Fields

    private WeakReference _cloneParentWeak;

    #endregion

    #region Ctors

    public SetterCollection() : base(new SetterCollectionRoot())
    {
      ((SetterCollectionRoot) Parent).Collection = this;
    }

    internal SetterCollection(SetterGroup setterGroup) : base(setterGroup)
    {
    }

    internal SetterCollection(FrameworkElement element) : base(GetElementRoot(element))
    {
    }

    private SetterCollection(IInteractivityObject interactivityRoot) : base(interactivityRoot)
    {
    }

    #endregion

    #region Properties

    private WeakLinkedList<SetterCollection> ActualClones => Clones ??= new WeakLinkedList<SetterCollection>();

    public SetterCollection CloneParent
    {
      get => _cloneParentWeak?.Target as SetterCollection;
      private set
      {
        if (ReferenceEquals(CloneParent, value))
          return;

        _cloneParentWeak = value != null ? new WeakReference(value) : null;
      }
    }

    private WeakLinkedList<SetterCollection> Clones { get; set; }

    private bool CloneSync { get; set; }

    protected override bool IsApplied => !(Parent is SetterGroup setterParent) || setterParent.IsApplied;

    #endregion

    #region  Methods

    internal void Load()
    {
	    Load((IInteractivityRoot)Parent);
		}

    internal void Unload()
    {
			Unload((IInteractivityRoot)Parent);
    }

    protected override void ClearCore()
    {
      EnsureCloneSync();

      base.ClearCore();

      if (Clones == null)
        return;

      try
      {
        CloneSync = true;

        foreach (var clone in Clones)
          if (ShouldSync(clone))
            clone.ClearCore();
      }
      finally
      {
        CloneSync = false;
      }
    }

    internal override InteractivityCollection<SetterBase> CreateInstance(IInteractivityObject parent)
    {
	    if (parent is SetterGroup setterGroup)
        return new SetterCollection(setterGroup);

	    if (parent is ElementRoot interactivityRoot)
        return new SetterCollection(interactivityRoot);

      return new SetterCollection();
    }

    internal SetterCollection DeepClone(FrameworkElement target)
    {
      var root = GetElementRoot(target);
      var clone = (SetterCollection)base.DeepClone(root);

      ActualClones.Add(clone);
      clone.CloneParent = this;
      clone.Load(root);

      return clone;
    }

    internal override InteractivityCollection<SetterBase> DeepClone(IInteractivityObject parent)
    {
      var clone = (SetterCollection)base.DeepClone(parent);

      ActualClones.Add(clone);
      clone.CloneParent = this;

      return clone;
    }

    private void EnsureCloneSync()
    {
      var cloneParent = CloneParent;

      if (cloneParent != null && cloneParent.CloneSync == false)
        cloneParent.ActualClones.Remove(this);
    }

    private static ElementRoot GetElementRoot(FrameworkElement element)
    {
      return InteractivityService.GetInteractivityService(element).ActualElementRoot;
    }

    protected override void InsertItemCore(int index, SetterBase item)
    {
      EnsureCloneSync();

      base.InsertItemCore(index, item);

      if (Clones == null)
        return;

      try
      {
        CloneSync = true;

        foreach (var clone in Clones)
          if (ShouldSync(clone))
            clone.Insert(index, item.DeepClone<SetterBase>());
      }
      finally
      {
        CloneSync = false;
      }
    }

    private void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
    {
      var setter = (SetterBase) descendants.Peek();
			var index = this.IndexOfReference(setter);

      EnsureCloneSync();

      if (Clones == null)
        return;

      try
      {
        CloneSync = true;

        foreach (var clone in Clones)
          if (ShouldSync(clone))
            clone[index] = setter.DeepClone<SetterBase>();
      }
      finally
      {
        CloneSync = false;
      }
    }

    protected override void RemoveItem(int index)
    {
      EnsureCloneSync();

      base.RemoveItem(index);

      if (Clones == null)
        return;

      try
      {
        CloneSync = true;

        foreach (var clone in Clones)
          if (ShouldSync(clone))
            ((IList<SetterBase>) clone).RemoveAt(index);
      }
      finally
      {
        CloneSync = false;
      }
    }

    protected override void SetItem(int index, SetterBase item)
    {
      EnsureCloneSync();

      base.SetItem(index, item);

      if (Clones == null)
        return;

      try
      {
        CloneSync = true;

        foreach (var clone in Clones)
          if (ShouldSync(clone))
            clone[index] = item.DeepClone<SetterBase>();
      }
      finally
      {
        CloneSync = false;
      }
    }

    private bool ShouldSync(SetterCollection clone)
    {
      return clone.Version + 1 == Version && ReferenceEquals(clone.CloneParent, this);
    }

    #endregion

    #region  Nested Types

    private class SetterCollectionRoot : InteractivityCollectionRoot<SetterCollection, SetterBase>
    {
      #region  Methods

      protected override void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName) => Collection?.OnDescendantApiPropertyChanged(descendants, propertyName);

      #endregion
    }

    #endregion
  }
}