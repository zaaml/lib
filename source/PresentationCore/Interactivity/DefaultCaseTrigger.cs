// <copyright file="DefaultCaseTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class DefaultCaseTrigger : CaseTriggerBase
	{
		#region  Methods

		protected override InteractivityObject CreateInstance()
		{
			return new DefaultCaseTrigger();
		}

		#endregion
	}
}