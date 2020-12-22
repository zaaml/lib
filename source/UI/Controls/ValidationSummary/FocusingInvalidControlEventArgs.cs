// <copyright file="FocusingInvalidControlEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.ValidationSummary
{
	public sealed class FocusingInvalidControlEventArgs : EventArgs
	{
		#region Ctors

		public FocusingInvalidControlEventArgs(ValidationSummaryItem item, ValidationSummarySource target)
		{
			Handled = false;
			Item = item;
			Target = target;
		}

		#endregion

		#region Properties

		public bool Handled { get; set; }


		public ValidationSummaryItem Item { get; }


		public ValidationSummarySource Target { get; set; }

		#endregion
	}
}
