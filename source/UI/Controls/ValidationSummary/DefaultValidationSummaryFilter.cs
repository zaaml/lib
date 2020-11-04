// <copyright file="DefaultValidationSummaryFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.ValidationSummary
{
	public class DefaultValidationSummaryFilter : ValidationSummaryFilter
	{
		#region Fields

		private ValidationSummaryItemTypeFilter _itemType;

		#endregion

		#region Properties

		public ValidationSummaryItemTypeFilter ItemType
		{
			get => _itemType;
			set
			{
				if (_itemType == value)
					return;

				_itemType = value;
				OnFilterChanged();
			}
		}

		#endregion

		#region  Methods

		protected override bool ShowInSummary(ValidationSummaryItem error)
		{
			return error.ItemType == ValidationSummaryItemType.ObjectError && (ItemType & ValidationSummaryItemTypeFilter.ObjectErrors) != ValidationSummaryItemTypeFilter.None ||
			       error.ItemType == ValidationSummaryItemType.PropertyError && (ItemType & ValidationSummaryItemTypeFilter.PropertyErrors) != ValidationSummaryItemTypeFilter.None;
		}

		#endregion
	}
}
