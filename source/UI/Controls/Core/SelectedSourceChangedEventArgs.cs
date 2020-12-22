// <copyright file="SelectedSourceChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Core
{
	public sealed class SelectedSourceChangedEventArgs : EventArgs
	{
		public readonly object NewSource;
		public readonly object OldSource;

		public SelectedSourceChangedEventArgs(object newValue, object oldValue)
		{
			NewSource = newValue;
			OldSource = oldValue;
		}
	}
}