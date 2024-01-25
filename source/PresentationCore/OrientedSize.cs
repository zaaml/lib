// <copyright file="OrientedSize.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using static System.Windows.Controls.Orientation;

namespace Zaaml.PresentationCore
{
	public struct OrientedSize
	{
		private Orientation _orientation;
		private double _indirect;
		private double _direct;


		public OrientedSize(Orientation orientation) : this()
		{
			_orientation = orientation;
		}

		public OrientedSize(Orientation orientation, double width, double height)
			: this()
		{
			_orientation = orientation;

			VerifySize(width, nameof(width));
			VerifySize(height, nameof(height));

			Width = width;
			Height = height;
		}

		private static void VerifySize(double value, string argumentName)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(argumentName);
		}

		public OrientedSize(Orientation orientation, Size size)
			: this()
		{
			_orientation = orientation;
			Width = size.Width;
			Height = size.Height;
		}


		public double Direct
		{
			get => _direct;
			set
			{
				VerifySize(value, nameof(value));

				_direct = value;
			}
		}

		public double Height
		{
			get => _orientation == Vertical ? Direct : Indirect;
			set
			{
				VerifySize(value, nameof(value));

				if (_orientation == Vertical)
					_direct = value;
				else
					_indirect = value;
			}
		}

		public double Indirect
		{
			get => _indirect;
			set
			{
				VerifySize(value, nameof(value));

				_indirect = value;
			}
		}

		public Orientation Orientation
		{
			get => _orientation;
			set
			{
				if (_orientation != value)
					Rotate();
			}
		}

		public Orientation IndirectOrientation => _orientation.Rotate();

		public Size Size
		{
			get => new Size(Width, Height);
			set => this = new OrientedSize(Orientation, value);
		}

		public double Width
		{
			get => _orientation == Horizontal ? Direct : Indirect;
			set
			{
				VerifySize(value, nameof(value));

				if (_orientation == Horizontal)
					_direct = value;
				else
					_indirect = value;
			}
		}

		public static OrientedSize operator +(OrientedSize first, OrientedSize second)
		{
			if (first.Orientation != second.Orientation)
				throw new InvalidOperationException("Orientations must match");

			return new OrientedSize(first.Orientation)
			{
				Direct = first.Direct + second.Direct,
				Indirect = first.Indirect + second.Indirect
			};
		}

		public static OrientedSize operator -(OrientedSize first, OrientedSize second)
		{
			if (first.Orientation != second.Orientation)
				throw new InvalidOperationException("Orientations must match");

			return new OrientedSize(first.Orientation)
			{
				Direct = (first.Direct - second.Direct).Clamp(0, double.PositiveInfinity),
				Indirect = (first.Indirect - second.Indirect).Clamp(0, double.PositiveInfinity)
			};
		}


		public double GetDirect(Size size)
		{
			return _orientation == Horizontal ? size.Width : size.Height;
		}

		public double GetIndirect(Size size)
		{
			return _orientation == Vertical ? size.Width : size.Height;
		}

		public OrientedSize ChangeDirect(double direct)
		{
			Direct = direct;

			return this;
		}

		public OrientedSize Clone => this;

		public OrientedSize ChangeIndirect(double indirect)
		{
			Indirect = indirect;

			return this;
		}

		public void Rotate()
		{
			(Direct, Indirect) = (Indirect, Direct);

			_orientation = _orientation.Rotate();
		}

		public static OrientedSize Create(Orientation orientation, double direct, double indirect)
		{
			return new OrientedSize(orientation) { Direct = direct, Indirect = indirect };
		}

		public static double GetDirect(Size size, Orientation orientation)
		{
			return orientation == Horizontal ? size.Width : size.Height;
		}

		public static double GetIndirect(Size size, Orientation orientation)
		{
			return orientation == Vertical ? size.Width : size.Height;
		}

		public override string ToString()
		{
			return $"Direct={Direct}, Indirect={Indirect}, Size={Size}";
		}
	}
}