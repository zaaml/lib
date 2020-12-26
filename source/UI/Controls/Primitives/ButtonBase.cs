// <copyright file="ButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
#if !SILVERLIGHT
using Zaaml.PresentationCore;
#endif

namespace Zaaml.UI.Controls.Primitives
{
	public abstract partial class ButtonBase : TemplateContractContentControl, IButton, IManagedButton, IIconOwner
	{
		public static readonly DependencyProperty CanClickProperty = DPM.Register<bool, ButtonBase>
			("CanClick", true);

		public static readonly DependencyProperty ClickModeProperty = DPM.Register<ClickMode, ButtonBase>
			("ClickMode", ClickMode.Release);

		public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, ButtonBase>
			("Command", g => g.OnCommandChanged);

		public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, ButtonBase>
			("CommandParameter", g => g.OnCommandParameterChanged);

		public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, ButtonBase>
			("CommandTarget", g => g.OnCommandTargetChanged);

		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, ButtonBase>
			("Icon", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

		public static readonly DependencyProperty IconDistanceProperty = DPM.Register<double, ButtonBase>
			("IconDistance", 4);

		public static readonly DependencyProperty IconDockProperty = DPM.Register<Dock, ButtonBase>
			("IconDock", Dock.Left);

		private static readonly DependencyPropertyKey IsPressedPropertyKey = DPM.RegisterReadOnly<bool, ButtonBase>
			("IsPressed", b => b.UpdateVisualState(true));

		public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ShowContentProperty = DPM.Register<bool, ButtonBase>
			("ShowContent", true);

		public static readonly DependencyProperty ShowIconProperty = DPM.Register<bool, ButtonBase>
			("ShowIcon", true);

		[UsedImplicitly] private readonly IButtonController _buttonController;

		static ButtonBase()
		{
#if !SILVERLIGHT
			KeyboardNavigation.AcceptsReturnProperty.OverrideMetadata(typeof(ButtonBase), new FrameworkPropertyMetadata(KnownBoxes.BoolTrue));
#endif
		}

		protected ButtonBase()
		{
			_buttonController = new ButtonController<ButtonBase>(this);
		}

		public bool CanClick
		{
			get => (bool) GetValue(CanClickProperty);
			set => SetValue(CanClickProperty, value);
		}

		public ClickMode ClickMode
		{
			get => (ClickMode) GetValue(ClickModeProperty);
			set => SetValue(ClickModeProperty, value);
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
			get => (bool) GetValue(ShowContentProperty);
			set => SetValue(ShowContentProperty, value);
		}

		public bool ShowIcon
		{
			get => (bool) GetValue(ShowIconProperty);
			set => SetValue(ShowIconProperty, value);
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new ButtonBaseTemplateContract();
		}

		protected virtual void OnClick()
		{
			RaiseClickEvent();
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

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);

			_buttonController.OnLostKeyboardFocus(e);
		}

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

		protected void RaiseClick()
		{
			_buttonController.RaiseOnClick();
		}

		public void UpdateCanExecute()
		{
			_buttonController.UpdateCanExecute();
		}

		public bool IsPressed
		{
			get => (bool) GetValue(IsPressedProperty);
			internal set => this.SetReadOnlyValue(IsPressedPropertyKey, value);
		}

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

		public IconBase Icon
		{
			get => (IconBase) GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		bool IManagedButton.IsMouseOver => IsMouseOver;

		ClickMode IManagedButton.ClickMode => ClickMode;

		bool IManagedButton.IsPressed
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		bool IManagedButton.CanClick => CanClick;

		bool IManagedButton.InvokeCommandBeforeClick => false;
		
		void IManagedButton.OnClick()
		{
			OnClick();
		}

		void IManagedButton.OnPreClick()
		{
		}

		void IManagedButton.OnPostClick()
		{
		}
		
		void IManagedButton.FocusControl()
		{
			if (Focusable)
				FocusHelper.SetKeyboardFocusedElement(this);
		}
	}

	public class ButtonBaseTemplateContract : TemplateContract
	{
	}
}