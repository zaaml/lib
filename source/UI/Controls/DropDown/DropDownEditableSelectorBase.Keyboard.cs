// <copyright file="DropDownEditableSelectorBase.Keyboard.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownEditableSelectorBase<TItemsControl, TItem>
	{
		private protected override DropDownSelectorKeyboardEventProcessor ActualKeyboardEventProcessor => EffectiveKeyboard ?? base.ActualKeyboardEventProcessor;

		private DropDownSelectorKeyboardEventProcessor EffectiveKeyboard { get; set; }

		private protected override bool HandleApplyCancelByKeyboard => IsEditing || base.HandleApplyCancelByKeyboard;

		private protected override MainKeyboardEventProcessor CreateMainKeyboard()
		{
			return new EditorMainKeyboardEventProcessor(this);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Tab && IsEditing && IsDropDownOpen)
			{
				EffectiveKeyboard = ActualKeyboardEventProcessor == ItemsKeyboard ? (DropDownSelectorKeyboardEventProcessor) MainKeyboard : ItemsKeyboard;

				e.Handled = true;

				return;
			}

			if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V)
			{
				if (IsEditing == false) 
					BeginEditDropDownSuspended();
			}

			base.OnPreviewKeyDown(e);
		}

		private protected class EditorMainKeyboardEventProcessor : MainKeyboardEventProcessor
		{
			public EditorMainKeyboardEventProcessor(DropDownEditableSelectorBase<TItemsControl, TItem> dropDownSelector) : base(dropDownSelector)
			{
			}

			public DropDownEditableSelectorBase<TItemsControl, TItem> Editor => (DropDownEditableSelectorBase<TItemsControl, TItem>) DropDownSelector;

			protected override void OnPreviewKeyDown(KeyEventArgs e)
			{
				if (Editor.IsEditing)
					if (e.Key == Key.Up || e.Key == Key.Down)
						Editor.EffectiveKeyboard = Editor.ItemsKeyboard;

				base.OnPreviewKeyDown(e);
			}
		}
	}
}