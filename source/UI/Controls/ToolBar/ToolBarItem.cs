// <copyright file="ToolBarItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.Overflow;

namespace Zaaml.UI.Controls.ToolBar
{
  [TemplateContractType(typeof(ToolBarItemTemplateContract))]
  public abstract class ToolBarItem : TemplateContractControl, IOverflowableItem<ToolBarItem>
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey IsOverflowPropertyKey = DPM.RegisterReadOnly<bool, ToolBarItem>
      ("IsOverflow", i => i.OnIsOverflowChanged);

    private static readonly DependencyPropertyKey ActualOrientationPropertyKey = DPM.RegisterReadOnly<Orientation, ToolBarItem>
      ("ActualOrientation");

    private static readonly DependencyPropertyKey ToolBarPropertyKey = DPM.RegisterReadOnly<ToolBarControl, ToolBarItem>
      ("ToolBar", i => i.OnToolBarChanged);

    public static readonly DependencyProperty ToolBarProperty = ToolBarPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsOverflowProperty = IsOverflowPropertyKey.DependencyProperty;
    public static readonly DependencyProperty ActualOrientationProperty = ActualOrientationPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private IDisposable _orientationBindingDisposer;
    public event EventHandler IsOverflowChanged;

    #endregion

    #region Ctors

    protected ToolBarItem()
    {
      OverflowController = new OverflowItemController<ToolBarItem>(this);
    }

    #endregion

    #region Properties

    public Orientation ActualOrientation => (Orientation) GetValue(ActualOrientationProperty);

    public bool IsOverflow
    {
      get => (bool) GetValue(IsOverflowProperty);
      internal set => this.SetReadOnlyValue(IsOverflowPropertyKey, value);
    }

    internal OverflowItemController<ToolBarItem> OverflowController { get; }

    public ToolBarControl ToolBar
    {
      get => (ToolBarControl) GetValue(ToolBarProperty);
      internal set => this.SetReadOnlyValue(ToolBarPropertyKey, value);
    }

    #endregion

    #region  Methods

    private void OnIsOverflowChanged()
    {
      IsOverflowChanged?.Invoke(this, EventArgs.Empty);
      OverflowController.IsOverflow = IsOverflow;
    }

    private void OnToolBarChanged()
    {
      _orientationBindingDisposer = _orientationBindingDisposer.DisposeExchange();

      if (ToolBar != null)
        _orientationBindingDisposer = this.BindPropertiesDisposable(ActualOrientationPropertyKey, ToolBar, ToolBarControl.ActualOrientationProperty);

      if (ToolBar != null)
        OverflowController.Attach();
      else
        OverflowController.Detach();
    }

    #endregion

    #region Interface Implementations

    #region IOverflowItem

    bool IOverflowableItem.IsOverflow => IsOverflow;

    #endregion

    #region IOverflowItem<ToolBarItem>

    OverflowItemController<ToolBarItem> IOverflowableItem<ToolBarItem>.OverflowController => OverflowController;

    #endregion

    #endregion
  }

  public class ToolBarItemTemplateContract : TemplateContract
  {
  }
}