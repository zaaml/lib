// <copyright file="EditingEndedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Editors.Core
{
	public sealed class EditingEndedEventArgs : EventArgs
	{
		internal static readonly EditingEndedEventArgs CommitArgs = new EditingEndedEventArgs(EditingResult.Commit);
		internal static readonly EditingEndedEventArgs CancelArgs = new EditingEndedEventArgs(EditingResult.Cancel);

		private EditingEndedEventArgs(EditingResult result)
		{
			Result = result;
		}

		public EditingResult Result { get; }
	}
}