// <copyright file="ArtboardSnapTarget.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapTarget : InheritanceContextObject
	{
		public static readonly DependencyProperty IsEnabledProperty = DPM.Register<bool, ArtboardSnapTarget>
			("IsEnabled", true);

		public static readonly DependencyProperty StrengthProperty = DPM.Register<double, ArtboardSnapTarget>
			("Strength", 100.0);

		public bool IsEnabled
		{
			get => (bool) GetValue(IsEnabledProperty);
			set => SetValue(IsEnabledProperty, value.Box());
		}

		public double Strength
		{
			get => (double) GetValue(StrengthProperty);
			set => SetValue(StrengthProperty, value);
		}

		public abstract IEnumerable<ArtboardSnapTargetPrimitive> GetSnapPrimitives(ArtboardSnapEngineContextParameters parameters);
	}
}