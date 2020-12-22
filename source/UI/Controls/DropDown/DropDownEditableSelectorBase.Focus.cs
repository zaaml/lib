// <copyright file="DropDownEditableSelectorBase.Focus.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Input;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownEditableSelectorBase<TItemsControl, TItem>
	{
		private bool ActualSuspendDropDownHandler => SuspendDropDownHandlerCount > 0;

		private bool ActualSuspendFocusHandler => IsSelectionHandling || SuspendFocusHandlerCount > 0;

		private bool IsEditorFocused => EditorCore != null && FocusHelper.IsKeyboardFocusWithin(EditorCore);

		private bool IsItemsControlFocused => ItemsControl != null && FocusHelper.IsKeyboardFocusWithin(ItemsControl);

		private bool IsSelfFocused => FocusHelper.IsKeyboardFocusWithin(this);

		private uint SuspendDropDownHandlerCount
		{
			get => PackedDefinition.SuspendDropDownHandlerCount.GetValue(_packedValue);
			set => PackedDefinition.SuspendDropDownHandlerCount.SetValue(ref _packedValue, value);
		}

		private uint SuspendFocusHandlerCount
		{
			get => PackedDefinition.SuspendFocusHandlerCount.GetValue(_packedValue);
			set => PackedDefinition.SuspendFocusHandlerCount.SetValue(ref _packedValue, value);
		}

		private bool SuspendOpenDropDown
		{
			get => PackedDefinition.SuspendOpenDropDown.GetValue(_packedValue);
			set => PackedDefinition.SuspendOpenDropDown.SetValue(ref _packedValue, value);
		}

		private void FocusEditor(bool updateLayout)
		{
			if (updateLayout)
				UpdateLayout();

			var editor = EditorCore;

			if (editor != null)
				FocusHelper.SetKeyboardFocusedElement(editor);
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			try
			{
				if (ActualSuspendFocusHandler)
					return;

				if (IsEditing == false)
				{
					BeginEditDropDownSuspended();

					if (IsEditing && IsTextEditable)
						FocusEditor(true);

					e.Handled = true;
				}

				if (IsEditing)
					e.Handled = true;
			}
			finally
			{
				UpdateVisualState(true);
			}
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			try
			{
				if (ActualSuspendFocusHandler)
					return;

				if (IsSelfFocused == false && IsEditorFocused == false && IsItemsControlFocused == false)
				{
					if (IsEditing)
					{
						CancelEditingPrivate();
						CancelSelection();
					}
				}
			}
			finally
			{
				UpdateVisualState(true);
			} 
		}

		private void ResumeDropDownHandler()
		{
			SuspendDropDownHandlerCount--;
		}

		private void ResumeFocusHandler()
		{
			SuspendFocusHandlerCount--;
		}

		private void SuspendDropDownHandler()
		{
			SuspendDropDownHandlerCount++;
		}

		private void SuspendFocusHandler()
		{
			SuspendFocusHandlerCount++;
		}
	}
}