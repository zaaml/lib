// <copyright file="PopupLength.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	[TypeConverter(typeof(PopupLengthTypeConverter))]
	public readonly struct PopupLength
	{
		public PopupLength(double unitValue, PopupLengthUnitType unitType)
		{
			_unitValue = unitValue;
			UnitType = unitType;
			RelativeElement = null;
		}

		public PopupLength(double unitValue, PopupLengthUnitType unitType, string relativeElement)
		{
			_unitValue = unitValue;
			UnitType = unitType;
			RelativeElement = relativeElement;
		}

		public PopupLength(double unitValue)
		{
			_unitValue = unitValue;
			UnitType = PopupLengthUnitType.Absolute;
			RelativeElement = null;
		}

		public static PopupLength Auto => new PopupLength(0.0, PopupLengthUnitType.Auto);

		private readonly double _unitValue;

		public PopupLengthUnitType UnitType { get; }

		public string RelativeElement { get; }

		public double Value => UnitType != PopupLengthUnitType.Auto ? _unitValue : 1.0;
	}
}