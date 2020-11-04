// <copyright file="SetterCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
  public abstract class SetterCollectionBase : InteractivityCollection<SetterBase>
  {
	  private bool _copyMembers;

    #region Ctors

    internal SetterCollectionBase(IInteractivityObject parent) : base(parent)
    {
    }

    #endregion

    #region Properties

    protected abstract bool IsApplied { get; }

    #endregion

    #region  Methods

    private void FixIndices(int startIndex, bool fromStart)
    {
      if (fromStart)
      {
        for (var iSetter = startIndex; iSetter < Count; iSetter++)
        {
          var setter = this[iSetter];
          // Usual setter collection will contain less than 65535 setters
          setter.Index = (ushort) iSetter.Clamp(0, ushort.MaxValue);
        }
      }
      else
      {
        for (var iSetter = Count - 1; iSetter >= startIndex; iSetter--)
        {
          var setter = this[iSetter];
          // Usual setter collection will contain less than 65535 setters
          setter.Index = (ushort) iSetter.Clamp(0, ushort.MaxValue);
        }
      }
    }

	  protected override void BeginCopyMembers()
	  {
		  base.BeginCopyMembers();

		  _copyMembers = true;
	  }

	  protected override void EndCopyMembers()
	  {
		  base.EndCopyMembers();

			_copyMembers = false;

			FixIndices(0, true);
	  }

	  protected override void InsertItemCore(int index, SetterBase item)
    {
      base.InsertItemCore(index, item);

			if (_copyMembers == false)
				FixIndices(index, false);

      LoadSetter(item);
    }

    private HashSet<string> _exceptProperties;

    private HashSet<string> ActualExceptProperties => _exceptProperties ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    internal void DisableSetters(IEnumerable<DependencyProperty> properties)
    {
      var isAny = properties.Aggregate(false, (current, dependencyProperty) => current | ActualExceptProperties.Add(dependencyProperty.GetQualifiedName()));

      if (isAny == false)
        return;

      var queue = new Queue<SetterBase>();

      foreach (var setter in this)
      {
        queue.Enqueue(setter);

        while (queue.Count > 0)
        {
          var currentSetter = queue.Dequeue();

          if (currentSetter is SetterGroup groupSetter)
          {
            foreach (var actualSetter in groupSetter.ActualSetters)
              queue.Enqueue(actualSetter);
          }
          else
          {
            var dependencyProperty = currentSetter.DependencyProperty;

            if (dependencyProperty == null)
              continue;

            if (ActualExceptProperties.Contains(dependencyProperty.GetQualifiedName()))
              currentSetter.IsOverriden = true;
          }
        }
      }
    }

    internal override void LoadCore(IInteractivityRoot root)
    {
      base.LoadCore(root);

      if (IsApplied == false) 
	      return;

      foreach (var setter in this)
        setter.Apply();
    }

    private void LoadSetter(SetterBase setter)
    {
      if (IsLoaded)
        setter.Load();

      if (IsApplied)
        setter.Apply();
    }

//#if !SILVERLIGHT
//		protected override void MoveItem(int oldIndex, int newIndex)
//		{
//			base.MoveItem(oldIndex, newIndex);

//			FixIndices(Math.Min(oldIndex, newIndex), false);
//		}
//#endif

    protected override void ClearCore()
    {
      foreach (var setter in this)
        UnloadSetter(setter);

      base.ClearCore();
    }

    protected override void RemoveItem(int index)
    {
      var item = this[index];

      base.RemoveItem(index);

      UnloadSetter(item);

	    if (_copyMembers == false)
				FixIndices(index, true);
    }

    protected override void SetItem(int index, SetterBase item)
    {
      var oldSetter = this[index];

      base.SetItem(index, item);

      var newSetter = this[index];

      UnloadSetter(oldSetter);
      LoadSetter(newSetter);
    }

    private void UnloadSetter(SetterBase setter)
    {
      if (setter.IsApplied)
        setter.Undo();

      if (setter.IsLoaded)
        setter.Unload();
    }

    #endregion
  }
}