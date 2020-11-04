// <copyright file="OverflowItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Primitives.Overflow
{
	public class OverflowItemCollection<TControl, TItem> : ItemCollectionBase<TControl, OverflowItem<TItem>> 
		where TItem : Control, IOverflowableItem<TItem> 
		where TControl : Control
	{
		#region Ctors

		public OverflowItemCollection(TControl control) : base(control)
		{
		}

		#endregion

		#region Properties

		protected override ItemGenerator<OverflowItem<TItem>> DefaultGenerator { get; } = new OverflowItemGenerator<TItem>();

		#endregion
	}
}