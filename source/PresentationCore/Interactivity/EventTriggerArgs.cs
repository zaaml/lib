// <copyright file="EventTriggerArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public class EventTriggerArgs
	{
		#region Static Fields and Constants

		public static EventTriggerArgs Instance = new EventTriggerArgs();

		#endregion

		#region Ctors

		private EventTriggerArgs()
		{
		}

		#endregion
	}
}