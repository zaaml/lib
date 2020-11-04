// <copyright file="ValidationSummaryItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.ValidationSummary
{
	public sealed class ValidationSummaryItemCollection : DependencyObjectCollectionBase<ValidationSummaryItem>
	{
		#region  Methods

		internal void ClearErrors(ValidationSummaryItemType errorType)
		{
			var validationItemCollection = new List<ValidationSummaryItem>();

			foreach (var validationSummaryItem in this)
			{
				if (validationSummaryItem != null && validationSummaryItem.ItemType == errorType)
					validationItemCollection.Add(validationSummaryItem);
			}

			foreach (var validationSummaryItem in validationItemCollection)
				Remove(validationSummaryItem);
		}

		#endregion
	}
}
