// <copyright file="ToggleGlyphBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Primitives
{
	public abstract class ToggleGlyphBase : GlyphBase, IReadOnlyControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IsCheckedProperty = DPM.Register<bool?, ToggleGlyphBase>
      ("IsChecked", c => c.OnIsCheckedChanged);

    public static readonly DependencyProperty IsReadOnlyProperty = DPM.Register<bool, ToggleGlyphBase>
      ("IsReadOnly");

    #endregion

    #region Properties

    public bool? IsChecked
    {
      get => (bool?) GetValue(IsCheckedProperty);
      set => SetValue(IsCheckedProperty, value);
    }

    #endregion

    #region  Methods

    private void OnIsCheckedChanged()
    {
      UpdateVisualState(true);
    }

    protected override void UpdateVisualState(bool useTransitions)
    {
      base.UpdateVisualState(useTransitions);

      switch (IsChecked)
      {
        case true:
          GotoVisualState("Checked", useTransitions);
          break;
        
        case false:
          GotoVisualState("Unchecked", useTransitions);
          break;
        
        default:
          GotoVisualState("Indeterminate", useTransitions);
          break;
      }
    }

    #endregion

    #region Interface Implementations

    #region IReadOnlyControl

    public bool IsReadOnly
    {
      get => (bool) GetValue(IsReadOnlyProperty);
      set => SetValue(IsReadOnlyProperty, value);
    }

    #endregion

    #endregion
  }
}