// <copyright file="SharedSizeEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.UI.Controls.Primitives.SharedSizePrimitives
{
  internal sealed class SharedSizeEntry
  {
    #region Fields

    private readonly WeakLinkedList<SharedSizeContentPanel> _registeredPanels = new WeakLinkedList<SharedSizeContentPanel>();

    private Size _actualSize;

    #endregion

    #region Ctors

    public SharedSizeEntry(string key, SharedSizeGroupControl sharedSizeGroupControl)
    {
      SharedSizeGroupControl = sharedSizeGroupControl;
      Key = key;
    }

    #endregion

    #region Properties

    public Size ActualSize
    {
      get => _actualSize;
      internal set
      {
        if (_actualSize.Equals(value))
          return;

        _actualSize = value;
        Generation++;
      }
    }

    internal long Generation { get; private set; }

    private bool IsDirty { get; set; }

    internal bool IsInMeasurePass { get; set; }

    public string Key { get; set; }

    internal SharedSizeGroupControl SharedSizeGroupControl { get; }

    #endregion

    #region  Methods

    internal void BeginMeasurePass(long generation)
    {
      IsInMeasurePass = true;

      Generation = generation;

      if (IsDirty)
      {
        var sharedPanel = SharedSizeGroupControl?.SharedSizeGroupPanel;
        foreach (var panel in EnumerateSharedPanels())
        {
          if (sharedPanel != null)
            sharedPanel.InvalidateSharedSizePanel(panel);
          else
            panel.InvalidateMeasure();
        }

        _actualSize = new Size();
      }

      IsDirty = false;
    }


    internal void EndMeasurePass()
    {
      IsInMeasurePass = false;
    }

    internal IEnumerable<SharedSizeContentPanel> EnumerateSharedPanels()
    {
      return _registeredPanels;
    }

    public void InvalidateGroup()
    {
      IsDirty = true;
      SharedSizeGroupControl.SharedSizeGroupPanel?.InvalidateInternal();
    }

    internal void NextMeasurePass(long generation)
    {
      Generation = generation;
    }

    internal void RegisterSharedPanel(SharedSizeContentPanel presenter)
    {
      _registeredPanels.Add(presenter);
    }

    public override string ToString()
    {
      return $"{Key} : {ActualSize}";
    }

    internal void UnregisterSharedPanel(SharedSizeContentPanel presenter)
    {
      _registeredPanels.Remove(presenter);
    }

    #endregion
  }
}