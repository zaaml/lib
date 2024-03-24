// <copyright file="DefaultTickBarItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.TickBar
{
	internal sealed class DefaultTickBarItemGenerator : TickBarItemGeneratorBase, IDelegatedGenerator<TickBarItem>
	{
		protected override void AttachItem(TickBarItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override TickBarItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(TickBarItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(TickBarItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		public IItemGenerator<TickBarItem> Implementation { get; set; }
	}
}