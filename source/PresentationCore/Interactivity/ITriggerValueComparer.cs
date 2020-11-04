// <copyright file="ITriggerValueComparer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public interface ITriggerValueComparer
	{
		#region  Methods

		bool Compare(object triggerSourceValue, object operand);

		#endregion
	}
}