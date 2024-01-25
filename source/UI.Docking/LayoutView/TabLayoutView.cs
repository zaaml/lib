// <copyright file="TabLayoutView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class TabLayoutView : TabLayoutViewBase<TabLayout>
	{
		public static readonly DependencyProperty HeaderPresenterProperty = DPM.Register<DockItemHeaderPresenter, TabLayoutView>
			("HeaderPresenter");

		static TabLayoutView()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TabLayoutView>();
		}

		public TabLayoutView()
		{
			this.OverrideStyleKey<TabLayoutView>();
		}

		public DockItemHeaderPresenter HeaderPresenter
		{
			get => (DockItemHeaderPresenter)GetValue(HeaderPresenterProperty);
			set => SetValue(HeaderPresenterProperty, value);
		}

		protected override int MinimumItemsCount => 2;
	}
}