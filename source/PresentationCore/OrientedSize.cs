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
		private double _direct;
		private double _indirect;
		private Orientation _orientation;

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

		public OrientedSize(Orientation orientation, Size size)
			: this()
		{
			_orientation = orientation;
			Width = size.Width;
			Height = size.Height;
		}

		public OrientedSize Clone => this;


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

		public Orientation IndirectOrientation => _orientation.Rotate();

		public Orientation Orientation
		{
			get => _orientation;
			set
			{
				if (_orientation != value)
					Rotate();
			}
		}

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

		public OrientedSize ChangeDirect(double direct)
		{
			Direct = direct;

			return this;
		}

		public OrientedSize ChangeIndirect(double indirect)
		{
			Indirect = indirect;

			return this;
		}

		public static OrientedSize Create(Orientation orientation, double direct, double indirect)
		{
			return new OrientedSize(orientation) { Direct = direct, Indirect = indirect };
		}


		public double GetDirect(Size size)
		{
			return _orientation == Horizontal ? size.Width : size.Height;
		}

		public static double GetDirect(Size size, Orientation orientation)
		{
			return orientation == Horizontal ? size.Width : size.Height;
		}

		public double GetIndirect(Size size)
		{
			return _orientation == Vertical ? size.Width : size.Height;
		}

		public static double GetIndirect(Size size, Orientation orientation)
		{
			return orientation == Vertical ? size.Width : size.Height;
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

		public static implicit operator Size(OrientedSize orientedSize)
		{
			return orientedSize.Size;
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

		public void Rotate()
		{
			(Direct, Indirect) = (Indirect, Direct);

			_orientation = _orientation.Rotate();
		}

		public override string ToString()
		{
			return $"Direct={Direct}, Indirect={Indirect}, Size={Size}";
		}

		private static void VerifySize(double value, string argumentName)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(argumentName);
		}

		public OrientedSize WithDirect(double direct)
		{
			return Clone.ChangeDirect(direct);
		}

		public OrientedSize WithIndirect(double indirect)
		{
			return Clone.ChangeIndirect(indirect);
		}
	}
}