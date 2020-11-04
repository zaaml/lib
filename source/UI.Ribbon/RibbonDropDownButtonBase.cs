// <copyright file="RibbonDropDownButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonDropDownButtonBaseTemplateContract))]
  public abstract class RibbonDropDownButtonBase : RibbonButtonBase, IDropDownControlHost
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty PopupControlProperty = DPM.Register<PopupControlBase, RibbonDropDownButtonBase>
      ("PopupControl", d => d.OnDropDownControlChanged);

    public static readonly DependencyProperty DropDownGlyphProperty = DPM.Register<IconBase, RibbonDropDownButtonBase>
      ("DropDownGlyph");

    public static readonly DependencyProperty IsDropDownOpenProperty = DPM.Register<bool, RibbonDropDownButtonBase>
      ("IsDropDownOpen", b => b.OnIsDropDownOpenChanged);

    public static readonly DependencyProperty PlacementProperty = DependencyPropertyManager.Register<Dock, RibbonDropDownButtonBase>
      ("Placement", Dock.Bottom);

    #endregion

    #region Fields

    [UsedImplicitly] private readonly DropDownPopupWrapper _dropDownPopupWrapper;
    private bool _isDropDownOpenInt;
    private object _logicalChild;

    public event EventHandler DropDownClosed;
    public event EventHandler DropDownOpened;

    #endregion

    #region Ctors

    protected RibbonDropDownButtonBase()
    {
      _dropDownPopupWrapper = new DropDownPopupWrapper(this, IsDropDownOpenProperty, PlacementProperty);

      LayoutUpdated += OnLayoutUpdated;
    }

    #endregion

    #region Properties

    internal virtual bool CanFocusOnClose => true;

    public IconBase DropDownGlyph
    {
      get => (IconBase) GetValue(DropDownGlyphProperty);
      set => SetValue(DropDownGlyphProperty, value);
    }

    public bool IsDropDownOpen
    {
      get => (bool) GetValue(IsDropDownOpenProperty);
      set => SetValue(IsDropDownOpenProperty, value);
    }

    private bool IsDropDownOpenInt
    {
      get => _isDropDownOpenInt;
      set
      {
        if (_isDropDownOpenInt == value)
          return;

        _isDropDownOpenInt = value;
        UpdateVisualState(true);
      }
    }

    protected override IEnumerator LogicalChildren => _logicalChild != null ? EnumeratorUtils.Concat(_logicalChild, base.LogicalChildren) : base.LogicalChildren;

    public Dock Placement
    {
      get => (Dock) GetValue(PlacementProperty);
      set => SetValue(PlacementProperty, value);
    }

    protected FrameworkElement PlacementTarget
    {
      get => _dropDownPopupWrapper.PlacementTarget;
      set => _dropDownPopupWrapper.PlacementTarget = value;
    }

    public PopupControlBase PopupControl
    {
      get => (PopupControlBase) GetValue(PopupControlProperty);
      set => SetValue(PopupControlProperty, value);
    }

    private PopupControlHost PopupHost => TemplateContract.PopupHost;

    private RibbonDropDownButtonBaseTemplateContract TemplateContract => (RibbonDropDownButtonBaseTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    protected virtual void OnClosed()
    {
      DropDownClosed?.Invoke(this, EventArgs.Empty);
    }

    private void OnClosedCore()
    {
      if (CanFocusOnClose && FocusHelper.IsKeyboardFocusWithin(this))
        FocusHelper.SetKeyboardFocusedElement(this);

      OnClosed();
    }

    private void OnDropDownControlChanged(PopupControlBase oldControl, PopupControlBase newControl)
    {
      _dropDownPopupWrapper.Popup = newControl;

      DropDownControlHostHelper.OnDropDownControlChanged(this, oldControl, newControl);
    }

    private void OnIsDropDownOpenChanged()
    {
      if (IsDropDownOpen)
        OnOpenedCore();
      else
        OnClosedCore();

      UpdateVisualState(true);
    }

    private void OnLayoutUpdated(object sender, EventArgs eventArgs)
    {
      IsDropDownOpenInt = IsDropDownOpen;
    }

    protected virtual void OnOpened()
    {
      DropDownOpened?.Invoke(this, EventArgs.Empty);
    }

    private void OnOpenedCore()
    {
      OnOpened();
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      DropDownControlHostHelper.OnHostAttached(this);
    }

    protected override void OnTemplateContractDetaching()
    {
      DropDownControlHostHelper.OnHostDetaching(this);

      base.OnTemplateContractDetaching();
    }

    protected override void UpdateVisualState(bool useTransitions)
    {
      base.UpdateVisualState(useTransitions);

      GotoVisualState(IsDropDownOpen || IsDropDownOpenInt ? CommonVisualStates.PopupOpened : CommonVisualStates.PopupClosed, useTransitions);
    }

    #endregion

    #region Interface Implementations

    #region IDropDownControlHost

    object IDropDownControlHost.LogicalChild
    {
      get => _logicalChild;
      set
      {
        if (ReferenceEquals(_logicalChild, value))
          return;

        if (_logicalChild != null)
          RemoveLogicalChild(_logicalChild);

        _logicalChild = value;

        if (_logicalChild != null)
          AddLogicalChild(_logicalChild);
      }
    }

    PopupControlHost IDropDownControlHost.PopupHost => PopupHost;

    PopupControlBase IDropDownControlHost.PopupControl => PopupControl;

    #endregion

    #endregion
  }

  public class RibbonDropDownButtonBaseTemplateContract : RibbonButtonBaseTemplateContract
  {
    #region Properties

    [TemplateContractPart]
    public PopupControlHost PopupHost { get; [UsedImplicitly] private set; }

    #endregion
  }
}