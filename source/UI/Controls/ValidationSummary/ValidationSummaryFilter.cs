// <copyright file="ValidationSummaryFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.ValidationSummary
{
	public abstract class ValidationSummaryFilter : InheritanceContextObject
	{
		#region Fields

		public event EventHandler FilterChanged;

		#endregion

		#region  Methods

		protected virtual void OnFilterChanged()
		{
			FilterChanged?.Invoke(this, EventArgs.Empty);
		}

		protected abstract bool ShowInSummary(ValidationSummaryItem item);

		internal bool ShowInSummaryInt(ValidationSummaryItem item)
		{
			return ShowInSummary(item);
		}

		#endregion
	}
}
