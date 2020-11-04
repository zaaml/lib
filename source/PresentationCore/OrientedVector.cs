// <copyright file="OrientedVector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore
{
	public struct OrientedVector
	{
		private Orientation _orientation;

		#region Ctors

		public OrientedVector(Orientation orientation)
			: this()
		{
			_orientation = orientation;
		}

		public OrientedVector(Orientation orientation, double x, double y)
			: this()
		{
			_orientation = orientation;
			X = x;
			Y = y;
		}

		public OrientedVector(Orientation orientation, Vector vector)
			: this()
		{
			_orientation = orientation;
			X = vector.X;
			Y = vector.Y;
		}

		#endregion

		#region Properties

		public double Direct { get; set; }

		public double Indirect { get; set; }

		public Orientation Orientation
		{
			get => _orientation;
			set
			{
				if (_orientation == value)
					return;

				Rotate();
			}
		}

		public Vector Vector => new Vector(X, Y);

		public double X
		{
			get => _orientation.IsHorizontal() ? Direct : Indirect;
			set
			{
				if (_orientation.IsHorizontal())
					Direct = value;
				else
					Indirect = value;
			}
		}

		public double Y
		{
			get => _orientation.IsVertical() ? Direct : Indirect;
			set
			{
				if (_orientation.IsVertical())
					Direct = value;
				else
					Indirect = value;
			}
		}

		#endregion

		#region Methods

		public double GetDirect(Size size)
		{
			return _orientation.IsHorizontal() ? size.Width : size.Height;
		}

		public double GetIndirect(Size size)
		{
			return _orientation.IsVertical() ? size.Width : size.Height;
		}

		public void Rotate()
		{
			var t = Direct;

			Direct = Indirect;
			Indirect = t;

			_orientation = _orientation.Rotate();
		}

		#endregion
	}
}