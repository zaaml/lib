// <copyright file="ItemsPanelHostCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Panels.Core
{
	internal sealed class ItemsPanelHostCollection<TItem> : ItemHostCollection<TItem>
		where TItem : Control
	{
		#region Fields

		private readonly ItemsPanel<TItem> _panel;

		#endregion

		#region Ctors

		public ItemsPanelHostCollection(ItemsPanel<TItem> panel)
		{
			_panel = panel;
		}

		#endregion

		#region Properties

		private UIElementCollection Children => _panel.Children;

		#endregion

		#region  Methods

		protected override void ClearCore()
		{
			Children.Clear();
		}

		protected override void InitCore(ICollection<TItem> items)
		{
			Sync();
		}

		protected override void InsertCore(int index, TItem item)
		{
			Children.Insert(index, item);

			Sync();
		}

		protected override void RemoveAtCore(int index)
		{
			Children.RemoveAt(index);

			Sync();
		}

		private void Sync()
		{
			if (Items.SequenceEqual(Children.Cast<TItem>()))
				return;

			Children.Clear();

			foreach (var frameworkElement in Items)
				Children.Add(frameworkElement);
		}

		#endregion
	}
}