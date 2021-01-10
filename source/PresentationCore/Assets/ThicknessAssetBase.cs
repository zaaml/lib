// <copyright file="ThicknessAssetBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Assets
{
	public abstract class ThicknessAssetBase : AssetBase
	{
		private static readonly DependencyPropertyKey ActualThicknessPropertyKey = DPM.RegisterReadOnly<Thickness, ThicknessAssetBase>
			("ActualThickness");

		public static readonly DependencyProperty ActualThicknessProperty = ActualThicknessPropertyKey.DependencyProperty;

		public Thickness ActualThickness
		{
			get => (Thickness) GetValue(ActualThicknessProperty);
			protected set => this.SetReadOnlyValue(ActualThicknessPropertyKey, value);
		}
	}
}