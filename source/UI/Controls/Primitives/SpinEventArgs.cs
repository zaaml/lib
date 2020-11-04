// <copyright file="SpinEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives
{
	public class SpinEventArgs : EventArgs
	{
		#region Ctors

		public SpinEventArgs(SpinDirection direction)
		{
			Direction = direction;
		}

		#endregion

		#region Properties

		public SpinDirection Direction { get; }

		#endregion
	}
}