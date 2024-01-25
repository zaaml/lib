// <copyright file="VisualState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity.VSM
{
	public sealed class VisualState
	{
		public VisualState()
		{
		}

		public VisualState(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}