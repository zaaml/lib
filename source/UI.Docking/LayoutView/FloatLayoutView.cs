// <copyright file="FloatLayoutView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class FloatLayoutView : BaseLayoutView<FloatLayout>
	{
		public FloatLayoutView()
		{
			Controller = new FloatingDockWindowController(this);
		}

		private HashSet<DockItem> AddQueue { get; } = new();

		private FloatingDockWindowController Controller { get; }

		private HashSet<DockItem> RemoveQueue { get; } = new();

		private HashSet<DockItem> UpdateQueue { get; } = new();

		protected internal override void ArrangeItems()
		{
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			if (AddQueue.Any() || RemoveQueue.Any())
				InvalidateMeasure();

			return base.ArrangeOverride(arrangeBounds);
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new FloatLayoutControlTemplateContract();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			// Remove windows
			foreach (var dockItem in RemoveQueue)
				Controller.DetachWindow(dockItem);

			// Add windows
			foreach (var dockItem in AddQueue)
				Controller.AttachWindow(dockItem);

			// Update windows
			foreach (var dockItem in UpdateQueue)
				Controller.UpdateWindow(dockItem);

			Controller.ArrangeWindows();

			AddQueue.Clear();
			RemoveQueue.Clear();
			UpdateQueue.Clear();

			return base.MeasureOverride(availableSize);
		}

		protected override void OnItemAdded(DockItem dockItem)
		{
			if (RemoveQueue.Remove(dockItem) == false)
				AddQueue.Add(dockItem);
			else
				UpdateQueue.Add(dockItem);

			dockItem.FloatingWindow?.AttachContent();

			InvalidateMeasure();
		}

		protected override void OnItemRemoved(DockItem dockItem)
		{
			UpdateQueue.Remove(dockItem);

			if (AddQueue.Remove(dockItem) == false)
				RemoveQueue.Add(dockItem);

			dockItem.FloatingWindow?.DetachContent();

			InvalidateMeasure();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			AddQueue.AddRange(Items.Except(RemoveQueue));
		}

		protected override void OnTemplateContractDetaching()
		{
			RemoveQueue.AddRange(Items.Except(AddQueue));

			base.OnTemplateContractDetaching();
		}
	}
}