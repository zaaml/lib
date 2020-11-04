// <copyright file="DefaultGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class DefaultGeneratorImpl<TItem, TGenerator> : IItemGenerator<TItem> where TItem : FrameworkElement where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		#region Ctors

		protected DefaultGeneratorImpl()
		{
			Generator = new TGenerator { Implementation = this };
		}

		#endregion

		#region Properties

		public TGenerator Generator { get; }

		#endregion

		#region Interface Implementations

		#region IItemGenerator<TItem>

		public abstract void AttachItem(TItem item, object itemSource);

		public abstract TItem CreateItem(object itemSource);

		public abstract void DetachItem(TItem item, object itemSource);

		public abstract void DisposeItem(TItem item, object itemSource);

		#endregion

		#endregion
	}

	internal interface IDelegatedGenerator<TItem> where TItem : FrameworkElement
	{
		#region Properties

		IItemGenerator<TItem> Implementation { set; }

		#endregion
	}
}