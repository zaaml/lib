// <copyright file="NavigationViewHeaderedIconItemPresenter.ButtonController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.NavigationView
{
	public partial class NavigationViewHeaderedIconItemPresenter : IManagedButton
	{
		[UsedImplicitly] private readonly IButtonController _buttonController;

		private bool IsPressed { get; set; }

		internal void OnCommandChanged()
		{
			_buttonController.OnCommandChanged();
		}

		internal void OnCommandParameterChanged()
		{
			_buttonController.OnCommandParameterChanged();
		}

		internal void OnCommandTargetChanged()
		{
			_buttonController.OnCommandTargetChanged();
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

		ICommand ICommandControl.Command => NavigationViewItem?.CommandInternal;

		object ICommandControl.CommandParameter => NavigationViewItem?.CommandParameterInternal;

		DependencyObject ICommandControl.CommandTarget => NavigationViewItem?.CommandTargetInternal;

		bool IManagedButton.CanClick => IsEnabled;

		ClickMode IManagedButton.ClickMode => NavigationViewItem?.ClickModeInternal ?? ClickMode.Press;

		bool IManagedButton.IsMouseOver => IsMouseOver;

		bool IManagedButton.IsPressed
		{
			get => IsPressed;
			set
			{
				IsPressed = value;

				if (NavigationViewItem != null)
					NavigationViewItem.IsPressedInternal = value;

				UpdateVisualState(true);
			}
		}

		void IManagedButton.OnPostClick()
		{
		}

		void IManagedButton.FocusControl()
		{
			FocusHelper.SetKeyboardFocusedElement(this);
		}

		bool IManagedButton.InvokeCommandBeforeClick => false;

		void IManagedButton.OnClick()
		{
			NavigationViewItem?.OnClickInternal();
		}

		void IManagedButton.OnPreClick()
		{
		}
	}
}