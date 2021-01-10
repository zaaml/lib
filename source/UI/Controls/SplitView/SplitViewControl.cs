// <copyright file="SplitViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.SplitView
{
	[TemplateContractType(typeof(SplitViewControlTemplateContract))]
	public class SplitViewControl : TemplateContractContentControl
	{
		public static readonly DependencyProperty IsPaneOpenProperty = DPM.Register<bool, SplitViewControl>
			("IsPaneOpen");

		public static readonly DependencyProperty DisplayModeProperty = DPM.Register<SplitViewDisplayMode, SplitViewControl>
			("DisplayMode", SplitViewDisplayMode.Inline);

		public static readonly DependencyProperty PaneProperty = DPM.Register<object, SplitViewControl>
			("Pane", d => d.LogicalChildMentor.OnLogicalChildPropertyChanged);

		private static readonly DependencyPropertyKey ActualPaneLengthPropertyKey = DPM.RegisterReadOnly<double, SplitViewControl>
			("ActualPaneLength");

		public static readonly DependencyProperty ActualPaneLengthProperty = ActualPaneLengthPropertyKey.DependencyProperty;

		public static readonly DependencyProperty CompactPaneLengthProperty = DPM.Register<double, SplitViewControl>
			("CompactPaneLength", 48);

		public static readonly DependencyProperty OpenPaneLengthProperty = DPM.Register<double, SplitViewControl>
			("OpenPaneLength", 320);

		public static readonly DependencyProperty PaneBackgroundProperty = DPM.Register<Brush, SplitViewControl>
			("PaneBackground");

		public static readonly DependencyProperty PanePlacementProperty = DPM.Register<SplitViewPanePlacement, SplitViewControl>
			("PanePlacement", SplitViewPanePlacement.Left);

		public static readonly DependencyProperty ShowPaneShadowProperty = DPM.Register<bool, SplitViewControl>
			("ShowPaneShadow", false);

		static SplitViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<SplitViewControl>();
		}

		public SplitViewControl()
		{
			this.OverrideStyleKey<SplitViewControl>();
		}

		public double ActualPaneLength
		{
			get => (double) GetValue(ActualPaneLengthProperty);
			private set => this.SetReadOnlyValue(ActualPaneLengthPropertyKey, value);
		}

		public double CompactPaneLength
		{
			get => (double) GetValue(CompactPaneLengthProperty);
			set => SetValue(CompactPaneLengthProperty, value);
		}

		private SplitViewContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		public SplitViewDisplayMode DisplayMode
		{
			get => (SplitViewDisplayMode) GetValue(DisplayModeProperty);
			set => SetValue(DisplayModeProperty, value);
		}

		public bool IsPaneOpen
		{
			get => (bool) GetValue(IsPaneOpenProperty);
			set => SetValue(IsPaneOpenProperty, value);
		}

		public double OpenPaneLength
		{
			get => (double) GetValue(OpenPaneLengthProperty);
			set => SetValue(OpenPaneLengthProperty, value);
		}

		public object Pane
		{
			get => GetValue(PaneProperty);
			set => SetValue(PaneProperty, value);
		}

		public Brush PaneBackground
		{
			get => (Brush) GetValue(PaneBackgroundProperty);
			set => SetValue(PaneBackgroundProperty, value);
		}

		private Panel PanePanel => TemplateContract.PanePanel;

		public SplitViewPanePlacement PanePlacement
		{
			get => (SplitViewPanePlacement) GetValue(PanePlacementProperty);
			set => SetValue(PanePlacementProperty, value);
		}

		private SplitViewPanePresenter PanePresenter => TemplateContract.PanePresenter;

		public bool ShowPaneShadow
		{
			get => (bool) GetValue(ShowPaneShadowProperty);
			set => SetValue(ShowPaneShadowProperty, value);
		}

		private SplitViewControlTemplateContract TemplateContract => (SplitViewControlTemplateContract) TemplateContractInternal;

		private void OnPanePanelSizeChanged(object sender, SizeChangedEventArgs e)
		{
			switch (PanePlacement)
			{
				case SplitViewPanePlacement.Top:
				case SplitViewPanePlacement.Bottom:

					ActualPaneLength = PanePanel.ActualHeight;
					break;

				case SplitViewPanePlacement.Left:
				case SplitViewPanePlacement.Right:

					ActualPaneLength = PanePanel.ActualWidth;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			PanePanel.SizeChanged += OnPanePanelSizeChanged;
		}

		protected override void OnTemplateContractDetaching()
		{
			PanePanel.SizeChanged -= OnPanePanelSizeChanged;

			base.OnTemplateContractDetaching();
		}
	}
}