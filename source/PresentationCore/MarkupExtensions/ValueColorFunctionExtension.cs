// <copyright file="ValueColorFunctionExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public abstract class ValueColorFunctionExtension : ColorFunctionExtension
	{
		public double Amount { get; set; }

		public ColorFunctionUnits Unit { get; set; }

		internal Core.ColorModel.ColorFunctionUnits CoreUnits => (Core.ColorModel.ColorFunctionUnits)Unit;
	}
}