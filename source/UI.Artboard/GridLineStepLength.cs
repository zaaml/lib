// <copyright file="GridLineStepLength.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public readonly struct GridLineStepLength
	{
		#region Fields

		public readonly GridLineStepUnit Unit;
		public readonly double Value;

		#endregion

		#region Ctors

		public GridLineStepLength(double value, GridLineStepUnit unit)
		{
			Value = value;
			Unit = unit;
		}

		#endregion
	}
}