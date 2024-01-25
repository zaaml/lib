// <copyright file="ListViewItem.Command.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.ListView
{
	public partial class ListViewItem : IManagedCommandItem
	{
		private bool _isPressed;

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
		}

		internal void OnCommandParameterChanged()
		{
		}

		internal void OnCommandTargetChanged()
		{
		}

		private void OnPostClick()
		{
			ListViewControl?.OnItemPostClick(this);
		}

		private void OnPreClick()
		{
			ListViewControl?.OnItemPreClick(this);
		}

		protected void RaiseClick()
		{
			ListViewControl?.RaiseClick(this);
		}

		internal void RaiseClickInternal()
		{
			RaiseClick();
		}

		ICommand ICommandControl.Command => Command;

		object ICommandControl.CommandParameter => CommandParameter;

		DependencyObject ICommandControl.CommandTarget => CommandTarget;

		bool IManagedCommandItem.CanClick => IsEnabled;

		ItemClickMode IManagedCommandItem.ClickMode => ListViewControl?.ItemClickMode ?? ItemClickMode.DoubleClick;

		bool IManagedCommandItem.IsMouseOver => IsMouseOver;

		bool IManagedCommandItem.IsPressed
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		bool IManagedCommandItem.InvokeCommandBeforeClick => false;

		void IManagedCommandItem.OnClick()
		{
			OnClick();
		}

		void IManagedCommandItem.OnPreClick()
		{
			OnPreClick();
		}

		void IManagedCommandItem.OnPostClick()
		{
			OnPostClick();
		}

		void IManagedCommandItem.FocusControl()
		{
			if (ListViewControl == null)
				FocusHelper.SetKeyboardFocusedElement(this);
			else if (ListViewControl.ActualFocusItemOnClickInternal)
				ListViewControl.FocusItemInternal(this);
		}
	}
}