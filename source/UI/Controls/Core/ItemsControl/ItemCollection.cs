// <copyright file="ItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Core
{
	public sealed class ItemCollection : ItemCollectionBase<ItemsControl, NativeControl>
	{
		#region Ctors

		internal ItemCollection(ItemsControl itemsControl) : base(itemsControl)
		{
		}

		#endregion

		#region Properties

		protected override ItemGenerator<NativeControl> DefaultGenerator => null;

		internal ItemGenerator<NativeControl> Generator
		{
			set => GeneratorCore = value;
		}

		#endregion
	}
}