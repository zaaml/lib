// <copyright file="IVisualStateListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IVisualStateListener
	{
		#region Properties

		string VisualStateName { get; }

		#endregion

		#region  Methods

		void EnterState(bool useTransitions);
		void LeaveState(bool useTransitions);

		#endregion
	}
}