// <copyright file="ValidationSummaryItemSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;

namespace Zaaml.UI.Controls.ValidationSummary
{
	public class ValidationSummaryItemSource
	{
		#region Ctors

		public ValidationSummaryItemSource(string propertyName)
			: this(propertyName, null)
		{
		}

		public ValidationSummaryItemSource(string propertyName, Control control)
		{
			PropertyName = propertyName;
			Control = control;
		}

		#endregion

		#region Properties

		public Control Control { get; }

		public string PropertyName { get; }

		#endregion

		#region  Methods

		public override bool Equals(object obj)
		{
			var summaryItemSource = obj as ValidationSummaryItemSource;
			if (summaryItemSource == null || PropertyName != summaryItemSource.PropertyName)
				return false;
			return Control == summaryItemSource.Control;
		}

		public override int GetHashCode()
		{
			return (PropertyName + "." + Control.Name).GetHashCode();
		}

		#endregion
	}
}
