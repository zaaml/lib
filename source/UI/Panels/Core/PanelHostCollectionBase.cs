// <copyright file="PanelHostCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Panels.Core
{
	internal abstract class PanelHostCollectionBase<TItem, TPanel> : ItemHostCollection<TItem>
		where TItem : FrameworkElement
		where TPanel : ItemsPanel<TItem>
	{
		protected PanelHostCollectionBase(TPanel panel)
		{
			Panel = panel;
		}

		protected virtual UIElementCollectionSpan Children => new(Panel.Children, 0, Panel.Children.Count);

		protected TPanel Panel { get; }

		protected virtual UIElement GetActualElement(TItem item)
		{
			return item;
		}

		private void SyncBase()
		{
			var index = 0;
			var count = Math.Min(Children.Count, Items.Count);

			for (; index < count; index++)
			{
				var item = GetActualElement(Items[index]);
				var child = Children[index];

				if (ReferenceEquals(item, child))
					continue;

				Children.RemoveRange(index, count - index);

				break;
			}

			if (index < Children.Count)
				Children.RemoveRange(index, Children.Count - index);

			for (; index < Items.Count; index++)
				Children.Add(GetActualElement(Items[index]));
		}

		protected override void SyncCore(SyncAction syncAction, SyncActionData syncActionData)
		{
			switch (syncAction)
			{
				case SyncAction.PreClear:
				case SyncAction.PreInit:

					Children.Clear();

					break;
				case SyncAction.PostInit:

					SyncBase();

					break;

				case SyncAction.PreInsert:

					SyncBase();

					break;
				case SyncAction.PostInsert:

					Children.Insert(syncActionData.Index, GetActualElement(syncActionData.Item));

					break;

				case SyncAction.PreRemove:

					SyncBase();

					break;
				case SyncAction.PostRemove:

					Children.RemoveAt(syncActionData.Index);

					break;
			}
		}
	}
}