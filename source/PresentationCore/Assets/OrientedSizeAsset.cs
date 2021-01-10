// <copyright file="OrientedSizeAsset.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Assets
{
	public sealed class OrientedSizeAsset : AssetBase
	{
		public static readonly DependencyProperty DirectProperty = DPM.Register<double, OrientedSizeAsset>
			("Direct", 0.0, d => d.OnDirectPropertyChangedPrivate);

		public static readonly DependencyProperty IndirectProperty = DPM.Register<double, OrientedSizeAsset>
			("Indirect", 0.0, d => d.OnIndirectPropertyChangedPrivate);

		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, OrientedSizeAsset>
			("Orientation", Orientation.Horizontal, d => d.OnOrientationPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ActualSizePropertyKey = DPM.RegisterReadOnly<Size, OrientedSizeAsset>
			("ActualSize");

		private static readonly DependencyPropertyKey ActualWidthPropertyKey = DPM.RegisterReadOnly<double, OrientedSizeAsset>
			("ActualWidth");

		private static readonly DependencyPropertyKey ActualHeightPropertyKey = DPM.RegisterReadOnly<double, OrientedSizeAsset>
			("ActualHeight");

		public static readonly DependencyProperty ActualHeightProperty = ActualHeightPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ActualSizeProperty = ActualSizePropertyKey.DependencyProperty;

		public double ActualHeight
		{
			get => (double) GetValue(ActualHeightProperty);
			private set => this.SetReadOnlyValue(ActualHeightPropertyKey, value);
		}

		public Size ActualSize
		{
			get => (Size) GetValue(ActualSizeProperty);
			private set => this.SetReadOnlyValue(ActualSizePropertyKey, value);
		}

		public double ActualWidth
		{
			get => (double) GetValue(ActualWidthProperty);
			private set => this.SetReadOnlyValue(ActualWidthPropertyKey, value);
		}

		public double Direct
		{
			get => (double) GetValue(DirectProperty);
			set => SetValue(DirectProperty, value);
		}

		public double Indirect
		{
			get => (double) GetValue(IndirectProperty);
			set => SetValue(IndirectProperty, value);
		}

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private void OnDirectPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateActualSize();
		}

		private void OnIndirectPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateActualSize();
		}

		private void OnOrientationPropertyChangedPrivate(Orientation oldValue, Orientation newValue)
		{
			UpdateActualSize();
		}

		private void UpdateActualSize()
		{
			var orientedSize = new OrientedSize(Orientation)
			{
				Direct = Direct,
				Indirect = Indirect
			};

			ActualSize = orientedSize.Size;
			ActualWidth = orientedSize.Size.Width;
			ActualHeight = orientedSize.Size.Height;
		}
	}
}