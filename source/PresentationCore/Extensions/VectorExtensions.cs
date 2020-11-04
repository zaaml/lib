// <copyright file="VectorExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
	internal static class VectorExtensions
	{
		#region  Methods

		public static OrientedVector AsOriented(this Vector vector, Orientation orientation)
		{
			return VectorUtils.AsOriented(vector, orientation);
		}

		#endregion
	}
}