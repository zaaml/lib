// <copyright file="IVisualStateListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IVisualStateListener
	{
		string VisualStateName { get; }

		void EnterState(bool useTransitions);

		void LeaveState(bool useTransitions);
	}
}