// <copyright file="ValueAssetBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	public abstract class ValueAssetBase<T> : AssetBase
	{
		public static readonly DependencyProperty ValueProperty = DPM.Register<T, ValueAssetBase<T>>
			("Value");

		public T Value
		{
			get => (T) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
	}

	public sealed class DoubleValueAsset : ValueAssetBase<double>
	{
	}

	public sealed class StringValueAsset : ValueAssetBase<string>
	{
	}

	public sealed class IntValueAsset : ValueAssetBase<int>
	{
	}

	public sealed class SizeValueAsset : ValueAssetBase<Size>
	{
	}

	public sealed class ThicknessValueAsset : ValueAssetBase<Thickness>
	{
	}

	public sealed class RectValueAsset : ValueAssetBase<Rect>
	{
	}
}