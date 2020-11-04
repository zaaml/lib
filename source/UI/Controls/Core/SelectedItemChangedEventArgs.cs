using System;

namespace Zaaml.UI.Controls.Core
{
	public sealed class SelectedItemChangedEventArgs<T> : EventArgs
	{
		#region Fields

		public readonly T NewItem;
		public readonly T OldItem;

		#endregion

		#region Ctors

		public SelectedItemChangedEventArgs(T newItem, T oldItem)
		{
			NewItem = newItem;
			OldItem = oldItem;
		}

		#endregion
	}

	public sealed class SelectedItemChangedEventArgs : EventArgs
	{
		#region Fields

		public readonly object NewItem;
		public readonly object OldItem;

		#endregion

		#region Ctors

		public SelectedItemChangedEventArgs(object newItem, object oldItem)
		{
			NewItem = newItem;
			OldItem = oldItem;
		}

		#endregion
	}
}