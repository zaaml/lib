// <copyright file="VirtualPanelLayoutBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Panels
{
	internal abstract class VirtualPanelLayoutBase<TPanel> : ScrollPanelLayoutBase<TPanel> 
		where TPanel : IPanel, IScrollViewPanel, IVirtualPanel
	{
		protected VirtualPanelLayoutBase(TPanel panel) : base(panel)
		{
		}

		protected int ItemsCount => Source?.Count ?? 0;

		protected IVirtualItemCollection Source => Panel.VirtualSource;

		public virtual void BringIntoView(BringIntoViewRequest request)
		{
		}
	}

	internal enum ItemLayoutInformationVisibility
	{
		Visible,
		Invisible,
		PartiallyVisible
	}

	internal readonly struct ItemLayoutInformation
	{
		public static readonly ItemLayoutInformation Empty = new ItemLayoutInformation(null, Rect.Empty, Rect.Empty);

		public ItemLayoutInformation(FrameworkElement item, Rect boundingBox, Rect panelBox)
		{
			Item = item;
			BoundingBox = boundingBox;
			PanelBox = panelBox;
		}

		public FrameworkElement Item { get; }

		public Rect BoundingBox { get; }

		public Rect PanelBox { get; }

		public ItemLayoutInformationVisibility Visibility
		{
			get
			{
				if (IsEmpty)
					return ItemLayoutInformationVisibility.Invisible;

				if (RectUtils.IntersectsWith(PanelBox, BoundingBox) == false)
					return ItemLayoutInformationVisibility.Invisible;

				if (RectUtils.Contains(PanelBox, BoundingBox))
					return ItemLayoutInformationVisibility.Visible;

				return ItemLayoutInformationVisibility.PartiallyVisible;
			}
		}

		public bool IsEmpty => Item == null;
	}
}