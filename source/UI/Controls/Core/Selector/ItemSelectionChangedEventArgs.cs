using System;

namespace Zaaml.UI.Controls.Core
{
	public class ItemSelectionChangedEventArgs<T> : EventArgs
	{
		public readonly T Item;
		public readonly bool IsSelected;

		public ItemSelectionChangedEventArgs(T item, bool isSelected)
		{
			Item = item;
			IsSelected = isSelected;
		}
	}
}