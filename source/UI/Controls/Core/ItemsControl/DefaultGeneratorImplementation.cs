// <copyright file="DefaultGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class DefaultGeneratorImplementation<TItem, TGenerator> : IItemGenerator<TItem>, IDelegatedItemGenerator<TItem>
		where TItem : FrameworkElement
		where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		protected DefaultGeneratorImplementation()
		{
			Generator = new TGenerator { Implementation = this };
		}

		public TGenerator Generator { get; }

		public abstract void AttachItem(TItem item, object source);

		public abstract TItem CreateItem(object source);

		public abstract void DetachItem(TItem item, object source);

		public abstract void DisposeItem(TItem item, object source);

		ItemGenerator<TItem> IDelegatedItemGenerator<TItem>.Implementation => Generator;
	}

	internal interface IDelegatedGenerator<TItem> where TItem : FrameworkElement
	{
		IItemGenerator<TItem> Implementation { set; }
	}

	internal interface IDelegatedItemGenerator<TItem>
		where TItem : FrameworkElement
	{
		ItemGenerator<TItem> Implementation { get; }
	}
}