// <copyright file="SplitDockItemGroupLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class SplitDockItemGroupLayout : DockItemGroupLayout
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, SplitDockItemGroupLayout>
			("Orientation", s => s.OnOrientationPropertyChangedPrivate);

		private static readonly DependencyProperty[] Properties =
		{
			OrientationProperty
		};

		public SplitDockItemGroupLayout(SplitDockItemGroup groupItem)
			: base(groupItem)
		{
			Orientation = groupItem.Orientation;
		}

		public SplitDockItemGroupLayout()
		{
		}

		internal SplitDockItemGroupLayout(SplitDockItemGroupLayout groupLayout, DockItemLayoutCloneMode mode) : base(groupLayout, mode)
		{
			if ((mode & DockItemLayoutCloneMode.BaseProperties) != 0)
				Orientation = groupLayout.Orientation;
		}

		internal override IEnumerable<DependencyProperty> FullLayoutProperties => base.FullLayoutProperties.Concat(OrientationProperty);

		internal override DockItemGroupKind GroupKind => DockItemGroupKind.Split;

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		internal override DockItemLayout CloneCore(DockItemLayoutCloneMode mode)
		{
			return new SplitDockItemGroupLayout(this, mode);
		}

		internal override IEnumerable<DependencyProperty> GetProperties()
		{
			return base.GetProperties().Concat(Properties);
		}

		internal override void InitGroup(DockItemGroup dockItemGroup)
		{
			((SplitDockItemGroup) dockItemGroup).Orientation = Orientation;
		}

		private void OnOrientationPropertyChangedPrivate()
		{
			OnLayoutPropertyChanged();
		}
	}
}