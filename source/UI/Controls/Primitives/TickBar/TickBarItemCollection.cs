// <copyright file="TickBarItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.TickBar
{
	public sealed class TickBarItemCollection : ItemCollectionBase<TickBarControl, TickBarItem>
	{
		public TickBarItemCollection(TickBarControl control) : base(control)
		{
		}

		protected override ItemGenerator<TickBarItem> DefaultGenerator { get; } = new TickBarItemGenerator();

		internal TickBarItemGeneratorBase Generator
		{
			get => (TickBarItemGeneratorBase)GeneratorCore;
			set => GeneratorCore = value;
		}
	}
}