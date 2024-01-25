// <copyright file="ToolBarButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.ToolBar
{
  [TemplateContractType(typeof(ToolBarButtonBaseTemplateContract))]
  public abstract partial class ToolBarButtonBase : ToolBarItem, IButton, IManagedButton, IIconOwner
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, ToolBarButtonBase>
      ("Command", g => g.OnCommandChanged);

    public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, ToolBarButtonBase>
      ("CommandParameter", g => g.OnCommandParameterChanged);

    public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, ToolBarButtonBase>
      ("CommandTarget", g => g.OnCommandTargetChanged);

    private static readonly DependencyPropertyKey IsPressedPropertyKey = DPM.RegisterReadOnly<bool, ToolBarButtonBase>
      ("IsPressed", b => b.UpdateVisualState(true));

    public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, ToolBarButtonBase>
      ("Icon", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

    public static readonly DependencyProperty IconDockProperty = DPM.Register<Dock, ToolBarButtonBase>
      ("IconDock", Dock.Left);

    public static readonly DependencyProperty IconDistanceProperty = DPM.Register<double, ToolBarButtonBase>
      ("IconDistance", 4);

    public static readonly DependencyProperty ContentProperty = DPM.Register<object, ToolBarButtonBase>
      ("Content");

    public static readonly DependencyProperty ContentTemplateProperty = DPM.Register<DataTemplate, ToolBarButtonBase>
      ("ContentTemplate");

    public static readonly DependencyProperty ClickModeProperty = DPM.Register<ClickMode, ToolBarButtonBase>
      ("ClickMode", ClickMode.Release);

    public static readonly DependencyProperty ShowIconProperty = DPM.Register<bool, ToolBarButtonBase>
      ("ShowIcon", true);

    public static readonly DependencyProperty ShowContentProperty = DPM.Register<bool, ToolBarButtonBase>
      ("ShowContent", true);

    public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    [UsedImplicitly] private readonly IButtonController _buttonController;

    #endregion

    #region Ctors

    static ToolBarButtonBase()
    {
#if !SILVERLIGHT
      KeyboardNavigation.AcceptsReturnProperty.OverrideMetadata(typeof(ToolBarButtonBase), new FrameworkPropertyMetadata(BooleanBoxes.True));
#endif
    }

    protected ToolBarButtonBase()
    {
      _buttonController = new ButtonController<ToolBarButtonBase>(this);
    }

    #endregion

    #region Properties

    internal bool CanClick { get; set; } = true;

    public ClickMode ClickMode
    {
      get => (ClickMode) GetValue(ClickModeProperty);
      set => SetValue(ClickModeProperty, value);
    }

    protected virtual bool CloseOverflowOnClick => true;

    public object Content
    {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
    }

    public DataTemplate ContentTemplate
    {
      get => (DataTemplate) GetValue(ContentTemplateProperty);
      set => SetValue(ContentTemplateProperty, value);
    }

    public double IconDistance
    {
      get => (double) GetValue(IconDistanceProperty);
      set => SetValue(IconDistanceProperty, value);
    }

    public Dock IconDock
    {
      get => (Dock) GetValue(IconDockProperty);
      set => SetValue(IconDockProperty, value);
    }

    public bool ShowContent
    {
	    get => (bool)GetValue(ShowContentProperty);
	    set => SetValue(ShowContentProperty, value.Box());
    }


    public bool ShowIcon
    {
	    get => (bool)GetValue(ShowIconProperty);
	    set => SetValue(ShowIconProperty, value.Box());
    }

    #endregion

    #region  Methods

    protected virtual void OnClick()
    {
      RaiseOnClick();
    }

    private void OnClickCore()
    {
      if (CloseOverflowOnClick)
        ToolBar?.CloseOverflow();

      OnClick();
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

    #region IIconOwner

    public IconBase Icon
    {
      get => (IconBase) GetValue(IconProperty);
      set => SetValue(IconProperty, value);
    }

    #endregion

    #region IManagedButton

    bool IManagedButton.IsMouseOver => IsMouseOver;

    ClickMode IManagedButton.ClickMode => ClickMode;

    bool IManagedButton.CanClick => CanClick;

    bool IManagedButton.IsPressed
    {
      get => IsPressed;
      set => IsPressed = value;
    }

		bool IManagedButton.InvokeCommandBeforeClick => false;

		void IManagedButton.OnClick()
    {
      OnClickCore();
    }

		void IManagedButton.OnPreClick()
		{
		}

		void IManagedButton.OnPostClick()
		{
		}
		
		void IManagedButton.FocusControl()
		{
		}
		
		#endregion

		#endregion
	}

	public class ToolBarButtonBaseTemplateContract : ToolBarItemTemplateContract
  {
  }
}