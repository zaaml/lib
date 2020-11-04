// <copyright file="StyleRuntimeSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal sealed class StyleRuntimeSetter : WeakRuntimeSetter
	{
		#region Properties

		public override long ActualPriority => 0x100000000 | Priority;

		#endregion
	}
}