// <copyright file="TreeViewItem.Command.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.TreeView
{
	public partial class TreeViewItem : IManagedCommandItem
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
			TreeViewControl?.OnItemPostClick(this);
		}

		private void OnPreClick()
		{
			TreeViewControl?.OnItemPreClick(this);
		}

		protected void RaiseClick()
		{
			TreeViewControl?.RaiseClick(this);
		}

		internal void RaiseClickInternal()
		{
			RaiseClick();
		}

		ICommand ICommandControl.Command => Command;

		object ICommandControl.CommandParameter => CommandParameter;

		DependencyObject ICommandControl.CommandTarget => CommandTarget;

		bool IManagedCommandItem.CanClick => IsEnabled;

		ClickMode IManagedCommandItem.ClickMode => TreeViewControl?.ItemClickMode ?? ClickMode.Release;

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
			if (TreeViewControl == null)
				FocusHelper.SetKeyboardFocusedElement(this);
			else if (TreeViewControl.ActualFocusItemOnClickInternal)
				TreeViewControl.FocusItemInternal(this);
		}
	}
}