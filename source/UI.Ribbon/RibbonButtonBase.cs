// <copyright file="RibbonButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonButtonBaseTemplateContract))]
  public abstract partial class RibbonButtonBase : RibbonItem, IButton, IManagedButton
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, RibbonButtonBase>
      ("Command", g => g.OnCommandChanged);

    public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, RibbonButtonBase>
      ("CommandParameter", g => g.OnCommandParameterChanged);

    public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, RibbonButtonBase>
      ("CommandTarget", g => g.OnCommandTargetChanged);

    private static readonly DependencyPropertyKey IsPressedPropertyKey = DPM.RegisterReadOnly<bool, RibbonButtonBase>
      ("IsPressed", b => b.UpdateVisualState(true));

    public static readonly DependencyProperty ClickModeProperty = DPM.Register<ClickMode, RibbonButtonBase>
      ("ClickMode", ClickMode.Release);

    public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    [UsedImplicitly] private readonly IButtonController _buttonController;

    #endregion

    #region Ctors

    static RibbonButtonBase()
    {
      KeyboardNavigation.AcceptsReturnProperty.OverrideMetadata(typeof(RibbonButtonBase), new FrameworkPropertyMetadata(true.Box()));
    }

    protected RibbonButtonBase()
    {
      _buttonController = new ButtonController<RibbonButtonBase>(this);
    }

    #endregion

    #region Properties

    internal bool CanClick { get; set; } = true;

    public ClickMode ClickMode
    {
      get => (ClickMode) GetValue(ClickModeProperty);
      set => SetValue(ClickModeProperty, value);
    }

    #endregion

    #region  Methods

    bool IManagedButton.InvokeCommandBeforeClick => false;

    protected virtual void OnClick()
    {
      RaiseOnClick();
    }

    void IManagedButton.OnPreClick()
    {
    }

    void IManagedButton.OnPostClick()
    {
    }

    private void OnCommandChanged()
    {
      _buttonController.OnCommandChanged();

      CommandChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnCommandParameterChanged()
    {
      _buttonController.OnCommandParameterChanged();

      CommandParameterChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnCommandTargetChanged()
    {
      _buttonController.OnCommandTargetChanged();

      CommandTargetChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);

      UpdateVisualState(true);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      _buttonController.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      base.OnKeyUp(e);

      _buttonController.OnKeyUp(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e);

#if SILVERLIGHT
      _buttonController.OnLostKeyboardFocus(e);
#endif
      UpdateVisualState(true);
    }

#if !SILVERLIGHT
    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnLostKeyboardFocus(e);

      _buttonController.OnLostKeyboardFocus(e);
    }
#endif

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      base.OnLostMouseCapture(e);

      _buttonController.OnLostMouseCapture(e);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      _buttonController.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);

      _buttonController.OnMouseLeave(e);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      _buttonController.OnMouseLeftButtonDown(e);

      base.OnMouseLeftButtonDown(e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      _buttonController.OnMouseLeftButtonUp(e);

      base.OnMouseLeftButtonUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      _buttonController.OnMouseMove(e);
    }

    #endregion

    #region Interface Implementations

    #region IButton

    public bool IsPressed
    {
      get => (bool) GetValue(IsPressedProperty);
      internal set => this.SetReadOnlyValue(IsPressedPropertyKey, value);
    }

    #endregion

    #region ICommandControl

    public event EventHandler CommandChanged;
    public event EventHandler CommandParameterChanged;
    public event EventHandler CommandTargetChanged;

    public DependencyObject CommandTarget
    {
      get => (DependencyObject) GetValue(CommandTargetProperty);
      set => SetValue(CommandTargetProperty, value);
    }

    public ICommand Command
    {
      get => (ICommand) GetValue(CommandProperty);
      set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
      get => GetValue(CommandParameterProperty);
      set => SetValue(CommandParameterProperty, value);
    }

    #endregion

    #region IManagedButton

    bool IManagedButton.CanClick => CanClick;

    bool IManagedButton.IsMouseOver => IsMouseOver;

    void IManagedButton.FocusControl()
    {
    }

    ClickMode IManagedButton.ClickMode => ClickMode;

    bool IManagedButton.IsPressed
    {
      get => IsPressed;
      set => IsPressed = value;
    }

    void IManagedButton.OnClick()
    {
      OnClick();
    }

    #endregion

    #endregion
  }

  public class RibbonButtonBaseTemplateContract : ButtonBaseTemplateContract
  {
  }
}