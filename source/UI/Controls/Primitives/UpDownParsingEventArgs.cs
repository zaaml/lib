// <copyright file="UpDownParsingEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives
{
	public class UpDownParsingEventArgs<T> : EventArgs
	{
		#region Ctors

		public UpDownParsingEventArgs(string text)
		{
			Text = text;
		}

		#endregion

		#region Properties

		public string Text { get; private set; }

		public T Value { get; set; }

		public bool Handled { get; set; }

		#endregion
	}
}