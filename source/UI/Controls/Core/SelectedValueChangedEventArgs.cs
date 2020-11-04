using System;

namespace Zaaml.UI.Controls.Core
{
	public sealed class SelectedValueChangedEventArgs : EventArgs
	{
		#region Fields

		public readonly object NewValue;
		public readonly object OldValue;

		#endregion

		#region Ctors

		public SelectedValueChangedEventArgs(object newValue, object oldValue)
		{
			NewValue = newValue;
			OldValue = oldValue;
		}

		#endregion
	}
}