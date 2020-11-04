// <copyright file="IVisualStateObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IVisualStateObserver
	{
		#region  Methods

		void AttachListener(IVisualStateListener listener);
		void DetachListener(IVisualStateListener listener);

		#endregion
	}
}