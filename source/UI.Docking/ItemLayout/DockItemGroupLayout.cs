// <copyright file="DockItemGroupLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	[ContentProperty(nameof(Items))]
	public abstract class DockItemGroupLayout : DockItemLayout, IDockItemLayoutCollectionOwner
	{
		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<DockItemLayoutCollection, DockItemGroupLayout>
			("ItemsPrivate");

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		protected DockItemGroupLayout(DockItemGroup groupItem)
			: base(groupItem)
		{
		}

		protected DockItemGroupLayout()
		{
		}

		internal DockItemGroupLayout(DockItemGroupLayout groupLayout, DockItemLayoutCloneMode mode) : base(groupLayout, mode)
		{
			if ((mode & DockItemLayoutCloneMode.Structure) != 0)
				CopyStructure(groupLayout, mode);
		}

		internal abstract DockItemGroupKind GroupKind { get; }

		internal override DockControlLayout Layout
		{
			get => base.Layout;
			set
			{
				base.Layout = value;

				foreach (var dockItemLayout in Items)
					dockItemLayout.Layout = value;
			}
		}

		private void CopyStructure(DockItemGroupLayout container, DockItemLayoutCloneMode mode)
		{
			foreach (var layout in container.Items)
				Items.Add(layout.Clone(mode));
		}

		private DockItemLayoutCollection CreateDockItemLayoutCollection()
		{
			return new DockItemLayoutCollection(this);
		}

		internal virtual void InitGroup(DockItemGroup dockItemGroup)
		{
		}

		public DockItemLayoutCollection Items => this.GetValueOrCreate(ItemsPropertyKey, CreateDockItemLayoutCollection);

		void IDockItemLayoutCollectionOwner.OnItemAdded(DockItemLayout dockItemLayout)
		{
			dockItemLayout.Layout = Layout;

			Layout?.OnLayoutChanged();
		}

		void IDockItemLayoutCollectionOwner.OnItemRemoved(DockItemLayout dockItemLayout)
		{
			if (ReferenceEquals(dockItemLayout.Layout, Layout) && Layout != null)
				dockItemLayout.Layout = null;

			Layout?.OnLayoutChanged();
		}
	}
}