// <copyright file="PopupBarItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Interfaces;
using NativeStyle = System.Windows.Style;
using ZaamlHeaderedContentControl = Zaaml.UI.Controls.Core.HeaderedContentControl;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public partial class PopupBarItem : ZaamlHeaderedContentControl, IManagedButton, IButton
	{
		public static readonly DependencyProperty IsSubBarOpenProperty = DPM.Register<bool, PopupBarItem>
			("IsSubBarOpen", false, p => DummyAction.Instance, p => p.CoerceIsSubBarOpenInt);

		public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, PopupBarItem>
			("Command", g => g.OnCommandChanged);

		public static readonly DependencyProperty PopupBarStyleProperty = DPM.Register<NativeStyle, PopupBarItem>
			("PopupBarStyle", p => p.OnPopupBarStyleChanged);

		public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, PopupBarItem>
			("CommandParameter", g => g.OnCommandParameterChanged);

		private static readonly DependencyPropertyKey IsPressedPropertyKey = DPM.RegisterReadOnly<bool, PopupBarItem>
			("IsPressed", false, b => b.UpdateVisualState(true));

		public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, PopupBarItem>
			("CommandTarget", g => g.OnCommandTargetChanged);

		public static readonly DependencyProperty AllowSubBarProperty = DPM.Register<bool, PopupBarItem>
			("AllowSubBar", true);

		public static readonly DependencyProperty PlacementOptionsProperty = DPM.Register<PopupPlacementOptions, PopupBarItem>
			("PlacementOptions", PopupPlacementOptions.FitMove);

		public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

		// ReSharper disable once NotAccessedField.Local
		private readonly IButtonController _buttonController;

		private PopupBar _popupBar;

		public event EventHandler CommandChanged;
		public event EventHandler CommandParameterChanged;
		public event EventHandler CommandTargetChanged;

		static PopupBarItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PopupBarItem>();
		}

		public PopupBarItem()
		{
			this.OverrideStyleKey<PopupBarItem>();

			_buttonController = new ButtonController<PopupBarItem>(this);
		}

		public bool AllowSubBar
		{
			get => (bool) GetValue(AllowSubBarProperty);
			set => SetValue(AllowSubBarProperty, value.Box());
		}

		public bool IsSubBarOpen
		{
			get => (bool) GetValue(IsSubBarOpenProperty);
			set => SetValue(IsSubBarOpenProperty, value.Box());
		}

		public PopupPlacementOptions PlacementOptions
		{
			get => (PopupPlacementOptions) GetValue(PlacementOptionsProperty);
			set => SetValue(PlacementOptionsProperty, value);
		}

		public NativeStyle PopupBarStyle
		{
			get => (NativeStyle) GetValue(PopupBarStyleProperty);
			set => SetValue(PopupBarStyleProperty, value);
		}

		private void ApplyPopupBarStyle()
		{
			if (_popupBar == null)
				return;

			if (this.HasLocalValue(PopupBarStyleProperty))
				_popupBar.SetValue(StyleProperty, PopupBarStyle);
			else
				_popupBar.ClearValue(StyleProperty);
		}

		protected virtual bool CoerceIsSubBarOpen(bool isSubBarOpen)
		{
			return isSubBarOpen;
		}

		private object CoerceIsSubBarOpenInt(object arg)
		{
			return AllowSubBar && CoerceIsSubBarOpen((bool) arg);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_popupBar = (PopupBar) GetTemplateChild("PopupBar");

			ApplyPopupBarStyle();
		}

		protected virtual void OnClick()
		{
			RaiseOnClick();
		}

		private void OnClickImpl()
		{
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

		private void OnPopupBarStyleChanged()
		{
			ApplyPopupBarStyle();
		}

		public bool IsPressed
		{
			get => (bool) GetValue(IsPressedProperty);
			internal set => this.SetReadOnlyValue(IsPressedPropertyKey, value);
		}

		void IManagedButton.FocusControl()
		{
			if (Focusable)
				FocusHelper.SetKeyboardFocusedElement(this);
		}

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

		bool IManagedButton.IsMouseOver => IsMouseOver;

		ClickMode IManagedButton.ClickMode => ClickMode.Press;

		bool IManagedButton.IsPressed
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		bool IManagedButton.CanClick => true;

		bool IManagedButton.InvokeCommandBeforeClick => false;

		void IManagedButton.OnClick()
		{
			OnClickImpl();
		}

		void IManagedButton.OnPreClick()
		{
		}

		void IManagedButton.OnPostClick()
		{
		}
	}
}