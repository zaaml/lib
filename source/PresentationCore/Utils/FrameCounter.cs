// <copyright file="FrameCounter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;

namespace Zaaml.PresentationCore.Utils
{
	internal static class FrameCounter
	{
		#region Ctors

		static FrameCounter()
		{
			CompositionTarget.Rendering += CompositionTargetOnRendering;
		}

		#endregion

		#region Properties

		public static long Frame { get; private set; }

		#endregion

		#region  Methods

		private static void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
		{
			Frame++;
		}

		#endregion
	}
}