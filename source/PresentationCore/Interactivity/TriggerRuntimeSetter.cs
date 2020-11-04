// <copyright file="TriggerRuntimeSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal sealed class TriggerRuntimeSetter : NonWeakRuntimeSetter
	{
		#region Properties

		public override long ActualPriority => 0x400000000 | RemoveOrder(Priority);

		#endregion
	}
}