// <copyright file="RibbonHeaderPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Ribbon
{
	public sealed class RibbonHeaderPanel : Panel
	{
		private readonly CategoriesHost _categoriesHost;
		private readonly PagesHost _pagesHost;

		private RibbonHeaderPresenter _presenter;

		public RibbonHeaderPanel()
		{
			_categoriesHost = new CategoriesHost(this);
			_pagesHost = new PagesHost(this);

			Children.Add(_categoriesHost);
			Children.Add(_pagesHost);
		}

		internal RibbonHeaderPresenter Presenter
		{
			get => _presenter;
			set
			{
				if (ReferenceEquals(_presenter, value))
					return;

				_presenter = value;

				_categoriesHost.Children.Clear();
				_pagesHost.Children.Clear();

				InvalidateMeasure();
			}
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return this.ArrangeStackLine(Orientation.Vertical, new Range<int>(0, 2), 0, 0, null, null).Size;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var size = new Size(availableSize.Width, double.PositiveInfinity);

			_pagesHost.Measure(size);

			if (Presenter.PageCategoriesPresenter != null)
				foreach (var category in Presenter.PageCategoriesPresenter.Items.ActualItemsInternal)
					category.UpdatePagesSize();

			_categoriesHost.Measure(size);

			return new OrientedSize(Orientation.Vertical).StackSize(_categoriesHost.DesiredSize).StackSize(_pagesHost.DesiredSize).Size;
		}

		private sealed class PagesHost : Panel, IFlexPanel
		{
			public PagesHost(RibbonHeaderPanel headerPanel)
			{
				HeaderPanel = headerPanel;
				Layout = new FlexPanelLayout(this);
			}

			private RibbonHeaderPanel HeaderPanel { get; }

			private FlexPanelLayout Layout { get; }

			private FrameworkElement Menu => Presenter?.Menu;

			private RibbonPagesPresenter PagesPresenter => Presenter?.PagesPresenter;

			private RibbonHeaderPresenter Presenter => HeaderPanel.Presenter;

			protected override Size ArrangeOverrideCore(Size finalSize)
			{
				return Layout.Arrange(finalSize);
			}

			protected override Size MeasureOverrideCore(Size availableSize)
			{
				Children.Clear();

				if (Menu != null)
					Children.Add(Menu);

				if (PagesPresenter != null)
					Children.Add(PagesPresenter);

				return Layout.Measure(availableSize);
			}

			IFlexDistributor IFlexPanel.Distributor => FlexDistributor.LastToFirst;

			bool IFlexPanel.HasHiddenChildren { get; set; }

			double IFlexPanel.Spacing => 0.0;

			FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

			FlexElement IFlexPanel.GetFlexElement(UIElement child)
			{
				// Menu
				if (ReferenceEquals(Menu, child))
					return new FlexElement { StretchDirection = FlexStretchDirection.None };

				// PagesPresenter
				if (ReferenceEquals(PagesPresenter, child))
					return new FlexElement { StretchDirection = FlexStretchDirection.Shrink };

				return child.GetFlexElement(this);
			}

			bool IFlexPanel.GetIsHidden(UIElement child)
			{
				return FlexPanel.GetIsHidden(child);
			}

			void IFlexPanel.SetIsHidden(UIElement child, bool value)
			{
				FlexPanel.SetIsHidden(child, value);
			}

			Orientation IOrientedPanel.Orientation => Orientation.Horizontal;
		}

		private sealed class CategoriesHost : Panel, IFlexPanel
		{
			private FlexLength _leftTitleLength;

			private FlexLength _qatLength;

			public CategoriesHost(RibbonHeaderPanel headerPanel)
			{
				Layout = new FlexPanelLayout(this);
				HeaderPanel = headerPanel;

				LayoutUpdated += OnLayoutUpdated;
			}

			private FrameworkElement FooterElement => Presenter?.FooterElement;

			private FrameworkElement HeaderElement => Presenter?.HeaderElement;

			private RibbonHeaderPanel HeaderPanel { get; }

			private FlexPanelLayout Layout { get; }

			private ContentPresenter LeftTitlePresenter { get; } = new ContentPresenter();

			private RibbonPageCategoriesPresenter PageCategoriesPresenter => Presenter?.PageCategoriesPresenter;

			private double PagesOffset { get; set; }

			private RibbonHeaderPresenter Presenter => HeaderPanel.Presenter;

			private RibbonToolBar QuickAccessToolBar => Presenter?.QuickAccessToolBar;

			private ContentPresenter RightTitlePresenter { get; } = new ContentPresenter();

			private double SelfOffset { get; set; }

			private FrameworkElement TitleElement => Presenter?.TitleElement;

			protected override Size ArrangeOverrideCore(Size finalSize)
			{
				return Layout.Arrange(finalSize);
			}

			private void InvalidatePagesCategoriesPanels()
			{
				var pagesPanel = Presenter?.Ribbon?.PagesPresenter?.ItemsHostInternal;
				var categoriesPanel = PageCategoriesPresenter?.ItemsHostInternal;

				pagesPanel?.InvalidateAncestorsMeasure(this);
				categoriesPanel?.InvalidateAncestorsMeasure(this);

				InvalidateMeasure();
			}

			protected override Size MeasureOverrideCore(Size availableSize)
			{
				Children.Clear();

				if (HeaderElement != null)
					Children.Add(HeaderElement);

				if (QuickAccessToolBar != null)
				{
					Children.Add(QuickAccessToolBar);
					QuickAccessToolBar.Measure(new Size(double.PositiveInfinity, availableSize.Height));
				}

				Children.Add(LeftTitlePresenter);

				if (PageCategoriesPresenter != null)
					Children.Add(PageCategoriesPresenter);

				Children.Add(RightTitlePresenter);

				if (FooterElement != null)
					Children.Add(FooterElement);

				LeftTitlePresenter.Content = null;
				RightTitlePresenter.Content = TitleElement;

				var measure = Size.Empty;

				for (var i = 0; i < 3; i++)
				{
					var categoriesOffset = PageCategoriesPresenter?.ItemsHostInternal?.Offset ?? 0.0;

					UpdateQatTitleLength();

					measure = Layout.Measure(availableSize);

					var newCategoriesOffset = PageCategoriesPresenter?.ItemsHostInternal?.Offset ?? 0.0;

					if (categoriesOffset.IsCloseTo(newCategoriesOffset))
						break;

					InvalidatePagesCategoriesPanels();
				}

				var leftTitleElement = Layout.GetActualElement(this, LeftTitlePresenter);
				var rightTitleElement = Layout.GetActualElement(this, RightTitlePresenter);

				if (leftTitleElement.ActualLength > rightTitleElement.ActualLength == false)
					return measure;

				RightTitlePresenter.Content = null;
				LeftTitlePresenter.Content = TitleElement;
				LeftTitlePresenter.Measure(new Size(leftTitleElement.ActualLength, LeftTitlePresenter.DesiredSize.Height));

				return measure;
			}

			private void OnLayoutUpdated(object sender, EventArgs e)
			{
				if (UpdateOffsets())
					return;

				InvalidatePagesCategoriesPanels();
			}

			private bool UpdateOffsets()
			{
				var pagesOffset = PagesOffset;
				var selfOffset = SelfOffset;

				var pagesPanel = Presenter?.Ribbon?.PagesPresenter?.ItemsHostInternal;
				var categoriesPanel = PageCategoriesPresenter?.ItemsHostInternal;

				if (pagesPanel != null && categoriesPanel != null)
				{
					PagesOffset = pagesPanel.TransformToAncestor(Presenter.Ribbon).Transform(new Point(0, 0)).X;
					SelfOffset = TransformToAncestor(Presenter.Ribbon).Transform(new Point(0, 0)).X;
				}

				return pagesOffset.IsCloseTo(PagesOffset) && selfOffset.IsCloseTo(SelfOffset);
			}

			private bool UpdateQatTitleLength()
			{
				var qatLength = _qatLength;
				var leftTitleLength = _leftTitleLength;

				var qatDesiredSize = QuickAccessToolBar?.DesiredSize.Width ?? 0.0;

				var pagesPanel = Presenter?.Ribbon?.PagesPresenter?.ItemsHostInternal;
				var categoriesPanel = PageCategoriesPresenter?.ItemsHostInternal;

				var updateOffsets = UpdateOffsets();

				if (pagesPanel != null && categoriesPanel != null)
				{
					var offset = PagesOffset - SelfOffset + categoriesPanel.Offset;

					_qatLength = new FlexLength(Math.Min(qatDesiredSize, offset), FlexLengthUnitType.Pixel);
					_leftTitleLength = new FlexLength(Math.Max(0, offset - _qatLength.Value), FlexLengthUnitType.Pixel);
				}
				else
				{
					_qatLength = FlexLength.Auto;
					_leftTitleLength = FlexLength.Auto;
				}

				return updateOffsets && qatLength.Equals(_qatLength) && leftTitleLength.Equals(_leftTitleLength);
			}

			FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

			public IFlexDistributor Distributor => FlexDistributor.LastToFirst;

			bool IFlexPanel.HasHiddenChildren { get; set; }

			double IFlexPanel.Spacing => 0;

			FlexElement IFlexPanel.GetFlexElement(UIElement child)
			{
				// Header
				if (ReferenceEquals(HeaderElement, child))
					return new FlexElement { StretchDirection = FlexStretchDirection.None };

				// Qat
				if (ReferenceEquals(QuickAccessToolBar, child))
					return new FlexElement { Length = _qatLength, StretchDirection = FlexStretchDirection.Shrink };

				// Left title
				if (ReferenceEquals(LeftTitlePresenter, child))
					return new FlexElement { Length = _leftTitleLength, StretchDirection = FlexStretchDirection.Both };

				// Categories
				if (ReferenceEquals(PageCategoriesPresenter, child))
					return new FlexElement { StretchDirection = FlexStretchDirection.Shrink };

				// Right title
				if (ReferenceEquals(RightTitlePresenter, child))
					return new FlexElement { Length = FlexLength.Star, StretchDirection = FlexStretchDirection.Shrink };

				// Footer
				if (ReferenceEquals(FooterElement, child))
					return new FlexElement { StretchDirection = FlexStretchDirection.None };

				return child.GetFlexElement(this);
			}

			bool IFlexPanel.GetIsHidden(UIElement child)
			{
				return FlexPanel.GetIsHidden(child);
			}

			void IFlexPanel.SetIsHidden(UIElement child, bool value)
			{
				FlexPanel.SetIsHidden(child, value);
			}

			Orientation IOrientedPanel.Orientation => Orientation.Horizontal;
		}
	}
}