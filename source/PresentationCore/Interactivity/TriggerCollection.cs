// <copyright file="TriggerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.PresentationCore.Interactivity
{
  public sealed class TriggerCollection : TriggerCollectionBase
  {
    #region Fields

    private WeakReference _cloneParentWeak;

    #endregion

    #region Ctors

    public TriggerCollection() : base(new TriggerCollectionRoot())
    {
      ((TriggerCollectionRoot) Parent).Collection = this;
    }

    internal TriggerCollection(FrameworkElement element) : base(GetElementRoot(element))
    {
		}

		internal TriggerCollection(IInteractivityObject parent) : base(parent)
    {
    }

    #endregion

    #region Properties

    private WeakLinkedList<TriggerCollection> ActualClones => Clones ??= new WeakLinkedList<TriggerCollection>();

    public TriggerCollection CloneParent
    {
      get => _cloneParentWeak?.Target as TriggerCollection;
      private set
      {
        if (ReferenceEquals(CloneParent, value))
          return;

        _cloneParentWeak = value != null ? new WeakReference(value) : null;
      }
    }

    private WeakLinkedList<TriggerCollection> Clones { get; set; }

    private bool CloneSync { get; set; }

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

    internal override InteractivityCollection<TriggerBase> CreateInstance(IInteractivityObject parent)
    {
	    if (parent is ElementRoot interactivityRoot)
        return new TriggerCollection(interactivityRoot);

      return new TriggerCollection(parent);
    }

    internal TriggerCollection DeepClone(FrameworkElement target)
    {
      var root = GetElementRoot(target);
      var clone = (TriggerCollection) base.DeepClone(root);

      ActualClones.Add(clone);
      clone.CloneParent = this;
      clone.Load(root);

      return clone;
    }

    internal override InteractivityCollection<TriggerBase> DeepClone(IInteractivityObject parent)
    {
      var clone = (TriggerCollection) base.DeepClone(parent);

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

    protected override void InsertItemCore(int index, TriggerBase item)
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
            clone.Insert(index, item.DeepClone<TriggerBase>());
      }
      finally
      {
        CloneSync = false;
      }
    }

    private void LoadTrigger(TriggerBase trigger)
    {
			if (IsLoaded)
				trigger.Load();

			trigger.IsEnabled = true;
		}

    private void UnloadTrigger(TriggerBase trigger)
    {
	    trigger.IsEnabled = false;

			if (trigger.IsLoaded)
				trigger.Unload();
		}

    private void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
    {
      var trigger = (TriggerBase) descendants.Peek();
			var index = this.IndexOfReference(trigger);

      EnsureCloneSync();

      if (Clones == null)
        return;

      try
      {
        CloneSync = true;

        foreach (var clone in Clones)
          if (ShouldSync(clone))
            clone[index] = trigger.DeepClone<TriggerBase>();
      }
      finally
      {
        CloneSync = false;
      }
    }

    protected override void OnItemAddedCore(TriggerBase item)
    {
      base.OnItemAddedCore(item);

			LoadTrigger(item);
    }

    protected override void OnItemRemovedCore(TriggerBase item)
    {
      UnloadTrigger(item);

      base.OnItemRemovedCore(item);
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
            ((IList<TriggerBase>) clone).RemoveAt(index);
      }
      finally
      {
        CloneSync = false;
      }
    }

    protected override void SetItem(int index, TriggerBase item)
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
            clone[index] = item.DeepClone<TriggerBase>();
      }
      finally
      {
        CloneSync = false;
      }
    }

    private bool ShouldSync(TriggerCollection clone)
    {
      return clone.Version + 1 == Version && ReferenceEquals(clone.CloneParent, this);
    }

    #endregion

    #region  Nested Types

    private class TriggerCollectionRoot : InteractivityCollectionRoot<TriggerCollection, TriggerBase>
    {
      #region  Methods

      protected override void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName) => Collection?.OnDescendantApiPropertyChanged(descendants, propertyName);

      #endregion
    }

    #endregion
  }
}