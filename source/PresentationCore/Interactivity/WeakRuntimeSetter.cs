// <copyright file="WeakRuntimeSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal abstract class WeakRuntimeSetter : RuntimeSetter
	{
		#region Properties

		public override bool IsWeak => true;

		#endregion
	}
}