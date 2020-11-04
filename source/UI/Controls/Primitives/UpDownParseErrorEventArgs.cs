// <copyright file="UpDownParseErrorEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives
{
	public class UpDownParseErrorEventArgs : EventArgs
	{
		#region Ctors

		public UpDownParseErrorEventArgs(string text, Exception error)
		{
			Text = text;
			Error = error;
		}

		#endregion

		#region Properties

		public string Text { get; private set; }

		public Exception Error { get; private set; }

		public bool Handled { get; set; }

		#endregion
	}
}