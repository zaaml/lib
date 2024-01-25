// <copyright file="IStateTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IStateTrigger<TStateTrigger, out TStateTriggerImplementation>
		where TStateTrigger : TriggerBase, IStateTrigger<TStateTrigger, TStateTriggerImplementation>
		where TStateTriggerImplementation : StateTriggerImplementation<TStateTrigger, TStateTriggerImplementation>
	{
		IEnumerable<InteractivityObject> BaseChildren { get; }
		TStateTriggerImplementation Implementation { get; }

		void DeinitializeTrigger(IInteractivityRoot root);

		void InitializeTrigger(IInteractivityRoot root);

		void OnClosed();

		void OnOpened();

		TriggerState UpdateState();
	}
}