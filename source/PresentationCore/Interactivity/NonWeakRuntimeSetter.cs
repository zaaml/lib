// <copyright file="NonWeakRuntimeSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal abstract class NonWeakRuntimeSetter : RuntimeSetter
	{
		#region Properties

		public override bool IsWeak => false;

		#endregion
	}
}