using System;

namespace Zaaml.UI.Controls.Core
{
	public class SelectedIndexChangedEventArgs : EventArgs
	{
		#region Fields

		public readonly int NewIndex;
		public readonly int OldIndex;

		#endregion

		#region Ctors

		public SelectedIndexChangedEventArgs(int newIndex, int oldIndex)
		{
			NewIndex = newIndex;
			OldIndex = oldIndex;
		}

		#endregion
	}
}