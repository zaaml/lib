// <copyright file="ToolBarDropDownButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.ToolBar
{
  public class ToolBarDropDownButton : ToolBarDropDownButtonBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ShowDropDownGlyphProperty = DPM.Register<bool, ToolBarDropDownButton>
      ("ShowDropDownGlyph", true);

    public static readonly DependencyProperty DropDownGlyphDockProperty = DPM.Register<Dock?, ToolBarDropDownButton>
      ("DropDownGlyphDock", Dock.Right);

    #endregion

    #region Ctors

    static ToolBarDropDownButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ToolBarDropDownButton>();
    }

    public ToolBarDropDownButton()
    {
      this.OverrideStyleKey<ToolBarDropDownButton>();
    }

    #endregion

    #region Properties

    protected override bool CloseOverflowOnClick => false;

    public Dock? DropDownGlyphDock
    {
      get => (Dock?) GetValue(DropDownGlyphDockProperty);
      set => SetValue(DropDownGlyphDockProperty, value);
    }

    public bool ShowDropDownGlyph
    {
      get => (bool) GetValue(ShowDropDownGlyphProperty);
      set => SetValue(ShowDropDownGlyphProperty, value);
    }

    #endregion

    #region  Methods

    protected override void OnClick()
    {
      base.OnClick();
      IsDropDownOpen = !IsDropDownOpen;
    }

    #endregion
  }
}