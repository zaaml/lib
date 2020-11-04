// <copyright file="ServiceRuntimeSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal sealed class ServiceRuntimeSetter : NonWeakRuntimeSetter
	{
		#region Properties

		public override long ActualPriority => 0x800000000 | RemoveOrder(Priority);

		#endregion
	}
}