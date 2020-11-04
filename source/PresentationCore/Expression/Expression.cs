// <copyright file="Expression.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	public static class Expression
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ScopeProperty = DPM.RegisterAttached<ExpressionScope>
			("Scope", typeof(Expression));

		#endregion

		#region  Methods

		[TypeConverter(typeof(ExpressionScopeTypeConverter))]
		public static ExpressionScope GetScope(DependencyObject element)
		{
			return (ExpressionScope) element.GetValue(ScopeProperty);
		}

		[TypeConverter(typeof(ExpressionScopeTypeConverter))]
		public static void SetScope(DependencyObject element, ExpressionScope value)
		{
			element.SetValue(ScopeProperty, value);
		}

		#endregion
	}
}