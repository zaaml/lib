// <copyright file="DockItemGroupLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;
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

		internal override XElement AsXElement(DockItemLayoutXElementOptions options)
		{
			var layoutXml = base.AsXElement(options);

			if (options.Structure == false)
				return layoutXml;

			foreach (var item in Items)
				layoutXml.Add(item.AsXElement(options));

			return layoutXml;
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

		protected override void OnDockStateChanged(DockItemState oldState, DockItemState newState)
		{
			foreach (var item in Items.GetByDockState(oldState).ToList())
				item.DockState = DockState;

			base.OnDockStateChanged(oldState, newState);
		}

		protected override void OnLayoutChanged(DockControlLayout oldLayout, DockControlLayout newLayout)
		{
			base.OnLayoutChanged(oldLayout, newLayout);

			foreach (var dockItemLayout in Items)
				dockItemLayout.Layout = newLayout;
		}

		public DockItemLayoutCollection Items => this.GetValueOrCreate(ItemsPropertyKey, CreateDockItemLayoutCollection);

		void IDockItemLayoutCollectionOwner.OnItemAdded(DockItemLayout dockItemLayout)
		{
			dockItemLayout.Layout = Layout;

			dockItemLayout.AttachGroup(DockState, this);

			Layout?.OnLayoutChanged();
		}

		void IDockItemLayoutCollectionOwner.OnItemRemoved(DockItemLayout dockItemLayout)
		{
			dockItemLayout.DetachGroup(this);

			if (ReferenceEquals(dockItemLayout.Layout, Layout) && Layout != null)
				dockItemLayout.Layout = null;

			Layout?.OnLayoutChanged();
		}
	}
}