// <copyright file="ValidationSummarySource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;

namespace Zaaml.UI.Controls.ValidationSummary
{
	public class ValidationSummarySource
	{
		public ValidationSummarySource(string propertyName)
			: this(propertyName, null)
		{
		}

		public ValidationSummarySource(string propertyName, Control control)
		{
			PropertyName = propertyName;
			Control = control;
		}

		public Control Control { get; }

		public string PropertyName { get; }

		public override bool Equals(object obj)
		{
			var summaryItemSource = obj as ValidationSummarySource;
			if (summaryItemSource == null || PropertyName != summaryItemSource.PropertyName)
				return false;
			return Control == summaryItemSource.Control;
		}

		public override int GetHashCode()
		{
			return (PropertyName + "." + Control.Name).GetHashCode();
		}
	}
}