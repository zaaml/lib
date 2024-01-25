// <copyright file="SplitDockItemGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class SplitDockItemGroup : DockItemGroup<SplitLayout>
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, SplitDockItemGroup>
			("Orientation", Orientation.Horizontal);

		static SplitDockItemGroup()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<SplitDockItemGroup>();
		}

		internal SplitDockItemGroup()
		{
			this.OverrideStyleKey<SplitDockItemGroup>();

			Layout.SetBinding(SplitLayout.OrientationProperty, new Binding
			{
				Source = this,
				Path = new PropertyPath(OrientationProperty),
				Mode = BindingMode.TwoWay
			});
		}

		public override DockItemGroupKind GroupKind => DockItemGroupKind.Split;

		public override DockItemKind Kind => DockItemKind.SplitDockItemGroup;

		protected override BaseLayoutView<SplitLayout> LayoutView => SplitLayoutView;

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private SplitLayoutView SplitLayoutView => TemplateContract.SplitLayoutView;

		private SplitDockItemGroupTemplateContract TemplateContract => (SplitDockItemGroupTemplateContract)TemplateContractInternal;

		protected internal override DockItemLayout CreateItemLayout()
		{
			return new SplitDockItemGroupLayout(this) { Orientation = Orientation };
		}

		protected override DockItem CreatePreviewItem(DockItemState dockState)
		{
			return new SplitDockItemGroup { DockState = dockState };
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new SplitDockItemGroupTemplateContract();
		}
	}
}