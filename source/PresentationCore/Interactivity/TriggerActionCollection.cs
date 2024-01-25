// <copyright file="TriggerActionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class TriggerActionCollection : InteractivityCollection<TriggerActionBase>
	{
		// ReSharper disable once SuggestBaseTypeForParameter
		public TriggerActionCollection(TriggerBase parent) : base(parent)
		{
		}

		internal override InteractivityCollection<TriggerActionBase> CreateInstance(IInteractivityObject parent)
		{
			return new TriggerActionCollection((TriggerBase)parent);
		}
	}
}