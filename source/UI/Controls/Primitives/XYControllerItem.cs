// <copyright file="XYControllerItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives
{
	public class XYControllerItem : ContentControl
	{
		public static readonly DependencyProperty XProperty = DPM.Register<double, XYControllerItem>
			("X", 0.0, d => d.OnXPropertyChangedPrivate, d => d.CoerceX);

		public static readonly DependencyProperty YProperty = DPM.Register<double, XYControllerItem>
			("Y", 0.0, d => d.OnYPropertyChangedPrivate, d => d.CoerceY);

		private static readonly DependencyPropertyKey XYControllerPropertyKey = DPM.RegisterReadOnly<XYController, XYControllerItem>
			("XYController", d => d.OnXYControllerPropertyChangedPrivate);

		public static readonly DependencyProperty XYControllerProperty = XYControllerPropertyKey.DependencyProperty;

		public double X
		{
			get => (double) GetValue(XProperty);
			set => SetValue(XProperty, value);
		}

		public XYController XYController
		{
			get => (XYController) GetValue(XYControllerProperty);
			internal set => this.SetReadOnlyValue(XYControllerPropertyKey, value);
		}

		public double Y
		{
			get => (double) GetValue(YProperty);
			set => SetValue(YProperty, value);
		}

		internal void Clamp()
		{
			var xyController = XYController;

			if (xyController == null)
				return;

			var x = X.Clamp(xyController.MinimumX, xyController.MaximumX);
			var y = Y.Clamp(xyController.MinimumY, xyController.MaximumY);

			if (x.IsCloseTo(X) == false)
				X = x;

			if (y.IsCloseTo(Y) == false)
				Y = y;
		}

		private double CoerceX(double x)
		{
			var xyController = XYController;

			if (xyController == null)
				return x;

			return x.Clamp(xyController.MinimumX, xyController.MaximumX);
		}

		private double CoerceY(double y)
		{
			var xyController = XYController;

			if (xyController == null)
				return y;

			return y.Clamp(xyController.MinimumY, xyController.MaximumY);
		}

		private void OnXPropertyChangedPrivate(double oldValue, double newValue)
		{
			XYController?.OnXChangedInternal(this, oldValue, newValue);
		}

		private void OnXYControllerPropertyChangedPrivate(XYController oldValue, XYController newValue)
		{
			Clamp();
		}

		private void OnYPropertyChangedPrivate(double oldValue, double newValue)
		{
			XYController?.OnYChangedInternal(this, oldValue, newValue);
		}
	}
}