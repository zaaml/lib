// <copyright file="TickBarSubDivisionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.TickBar
{
	public sealed class TickBarSubDivisionCollection : InheritanceContextDependencyObjectCollection<TickBarSubDivision>
	{
		internal TickBarSubDivisionCollection(TickBarControl tickBarControl)
		{
			TickBarControl = tickBarControl;
		}

		public TickBarControl TickBarControl { get; }

		protected override void OnItemAdded(TickBarSubDivision subDivision)
		{
			base.OnItemAdded(subDivision);

			subDivision.TickBarControl = TickBarControl;
		}

		protected override void OnItemRemoved(TickBarSubDivision subDivision)
		{
			subDivision.TickBarControl = null;

			base.OnItemRemoved(subDivision);
		}
	}
}