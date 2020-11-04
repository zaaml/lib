// <copyright file="VectorUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Zaaml.PresentationCore.Utils
{
	internal static class VectorUtils
	{
		#region  Methods

		public static OrientedVector AsOriented(Vector vector, Orientation orientation)
		{
			return new OrientedVector(orientation, vector);
		}

		#endregion
	}
}