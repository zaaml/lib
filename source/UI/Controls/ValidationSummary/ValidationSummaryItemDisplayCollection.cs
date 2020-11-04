// <copyright file="ValidationSummaryItemDisplayCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.ValidationSummary
{
	public sealed class ValidationSummaryItemDisplayCollection : ReadOnlyDependencyObjectCollection<ValidationSummaryItem>
	{
		#region Ctors

		internal ValidationSummaryItemDisplayCollection(ValidationSummaryItemCollection list) : base(list)
		{
		}

		#endregion
	}
}
