// <copyright file="DefaultRuntimeSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal sealed class DefaultRuntimeSetter : WeakRuntimeSetter
	{
		#region Properties

		public override long ActualPriority => 0x000000000 | Priority;

		#endregion
	}
}