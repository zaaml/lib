// <copyright file="CompositeThickness.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Assets
{
	public sealed class CompositeThickness : ThicknessAssetBase
	{
		public static readonly DependencyProperty LeftProperty = DPM.Register<double, CompositeThickness>
			("Left", double.NaN, b => b.UpdateActualThickness);

		public static readonly DependencyProperty TopProperty = DPM.Register<double, CompositeThickness>
			("Top", double.NaN, b => b.UpdateActualThickness);

		public static readonly DependencyProperty RightProperty = DPM.Register<double, CompositeThickness>
			("Right", double.NaN, b => b.UpdateActualThickness);

		public static readonly DependencyProperty BottomProperty = DPM.Register<double, CompositeThickness>
			("Bottom", double.NaN, b => b.UpdateActualThickness);

		public static readonly DependencyProperty ThicknessProperty = DPM.Register<Thickness, CompositeThickness>
			("Thickness", b => b.UpdateActualThickness);

		public double Bottom
		{
			get => (double) GetValue(BottomProperty);
			set => SetValue(BottomProperty, value);
		}

		public double Left
		{
			get => (double) GetValue(LeftProperty);
			set => SetValue(LeftProperty, value);
		}

		public double Right
		{
			get => (double) GetValue(RightProperty);
			set => SetValue(RightProperty, value);
		}

		public Thickness Thickness
		{
			get => (Thickness) GetValue(ThicknessProperty);
			set => SetValue(ThicknessProperty, value);
		}

		public double Top
		{
			get => (double) GetValue(TopProperty);
			set => SetValue(TopProperty, value);
		}

		private void UpdateActualThickness()
		{
			var thickness = Thickness;

			if (Left.IsNaN() == false)
				thickness.Left = Left;

			if (Top.IsNaN() == false)
				thickness.Top = Top;

			if (Right.IsNaN() == false)
				thickness.Right = Right;

			if (Bottom.IsNaN() == false)
				thickness.Bottom = Bottom;

			if (ActualThickness != thickness)
				ActualThickness = thickness;
		}
	}
}