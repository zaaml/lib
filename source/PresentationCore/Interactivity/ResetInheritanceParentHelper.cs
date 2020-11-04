// <copyright file="ResetInheritanceParentHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	public static class ResetInheritanceParentHelper
	{
		#region Static Fields and Constants

		private static readonly ResetObject ResetObjectInstance = new ResetObject();

		#endregion

		#region  Methods

		public static object ResetInheritanceParent(this object value)
		{
			var dpValue = value as DependencyObject;
			if (dpValue == null) return value;

			ResetObjectInstance.SetValue(ResetObject.ValueProperty, dpValue);
			ResetObjectInstance.ClearValue(ResetObject.ValueProperty);

			return value;
		}

		#endregion

		#region  Nested Types

		private class ResetObject : DependencyObject
		{
			#region Static Fields and Constants

			public static readonly DependencyProperty ValueProperty = DPM.Register<object, ResetObject>
				("Value");

			#endregion
		}

		#endregion
	}
}