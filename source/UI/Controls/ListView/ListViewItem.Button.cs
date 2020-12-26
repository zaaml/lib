// <copyright file="ListViewItem.Button.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.ListView
{
	public partial class ListViewItem : IManagedButton
	{
		private bool _isPressed;

		private IButtonController ButtonController { get; }

		private bool IsPressed
		{
			get => _isPressed;
			set
			{
				if (_isPressed == value)
					return;

				_isPressed = value;

				UpdateVisualState(true);
			}
		}

		internal void OnCommandChanged()
		{
			ButtonController.OnCommandChanged();
		}

		internal void OnCommandParameterChanged()
		{
			ButtonController.OnCommandParameterChanged();
		}

		internal void OnCommandTargetChanged()
		{
			ButtonController.OnCommandTargetChanged();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			ButtonController.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			ButtonController.OnKeyUp(e);
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);

			ButtonController.OnLostKeyboardFocus(e);
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);

			ButtonController.OnLostMouseCapture(e);
		}

		protected void RaiseClick()
		{
			ButtonController.RaiseOnClick();
		}

		public void UpdateCanExecute()
		{
			ButtonController.UpdateCanExecute();
		}

		ICommand ICommandControl.Command => Command;

		object ICommandControl.CommandParameter => CommandParameter;

		DependencyObject ICommandControl.CommandTarget => CommandTarget;

		bool IManagedButton.CanClick => IsEnabled;

		ClickMode IManagedButton.ClickMode => ListViewControl?.ItemClickMode ?? ClickMode.Release;

		bool IManagedButton.IsMouseOver => IsMouseOver;

		bool IManagedButton.IsPressed
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		bool IManagedButton.InvokeCommandBeforeClick => false;
		
		void IManagedButton.OnClick()
		{
			OnClick();
		}

		void IManagedButton.OnPreClick()
		{
			ListViewControl?.OnItemPreClick(this);
		}

		void IManagedButton.OnPostClick()
		{
			ListViewControl?.OnItemPostClick(this);
		}

		void IManagedButton.FocusControl()
		{
			if (ListViewControl == null)
				FocusHelper.SetKeyboardFocusedElement(this);
			else if (ListViewControl.ActualFocusItemOnClickInternal)
				ListViewControl.FocusItemInternal(this);
		}
	}
}