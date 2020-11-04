// <copyright file="TriggerActionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class TriggerActionBase : InteractivityObject
	{
		#region  Methods

		internal void Invoke()
		{
			if (IsLoaded)
				InvokeCore();
		}

		protected abstract void InvokeCore();

		#endregion
	}
}