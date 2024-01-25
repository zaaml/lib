// <copyright file="ValueAsset.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	[ContentProperty("Value")]
	public class ValueAsset : AssetBase
	{
		public static readonly DependencyProperty ValueProperty = DPM.Register<object, ValueAsset>
			("Value");

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
	}
}