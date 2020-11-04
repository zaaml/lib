// <copyright file="LocalRuntimeSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal sealed class LocalRuntimeSetter : NonWeakRuntimeSetter
	{
		#region Properties

		public override long ActualPriority => 0x200000000 | Priority;

		#endregion
	}
}