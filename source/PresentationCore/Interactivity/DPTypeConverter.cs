// <copyright file="DPTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
	internal class DPTypeConverter : TypeConverter
	{
		#region Static Fields and Constants

		public static readonly DPTypeConverter Instance = new DPTypeConverter();

		#endregion

		#region  Methods

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(DependencyProperty);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return DependencyPropertyProxyManager.GetDependencyProperty((string) value);
		}

		#endregion
	}
}