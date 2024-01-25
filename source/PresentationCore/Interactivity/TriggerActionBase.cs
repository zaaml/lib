// <copyright file="TriggerActionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class TriggerActionBase : InteractivityObject
	{
		internal void Invoke()
		{
			if (IsLoaded)
				InvokeCore();
		}

		protected abstract void InvokeCore();
	}
}