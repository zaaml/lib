// <copyright file="ZFrameworkPropertyMetadataOptions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.PropertyCore
{
	[Flags]
	internal enum ZFrameworkPropertyMetadataOptions
	{
		None = 0,
		AffectsMeasure = 1,
		AffectsArrange = 2,
		AffectsParentMeasure = 4,
		AffectsParentArrange = 8
	}
}