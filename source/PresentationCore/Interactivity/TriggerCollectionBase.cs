// <copyright file="TriggerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class TriggerCollectionBase : InteractivityCollection<TriggerBase>
	{
		#region Ctors

		internal TriggerCollectionBase(IInteractivityObject parent) : base(parent)
		{
		}

		#endregion
	}
}