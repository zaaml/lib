// <copyright file="IEventTriggerArgsSupport.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IEventTriggerArgsSupport
	{
		#region  Methods

		void SetArgs(EventArgs args);

		#endregion
	}
}