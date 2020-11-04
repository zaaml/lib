// <copyright file="ValidationSummaryFilters.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.ValidationSummary
{
	[Flags]
	public enum ValidationSummaryItemTypeFilter
	{
		None = 0,
		ObjectErrors = 1,
		PropertyErrors = 2,
		All = PropertyErrors | ObjectErrors
	}
}
