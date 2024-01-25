// <copyright file="DropCompass.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Docking
{
	public abstract class DropCompass : Control
	{
		public static readonly DependencyProperty AllowedActionsProperty = DPM.Register<DropGuideAction, DropCompass>
			("AllowedActions", c => c.OnAllowedActionsPropertyChanged);

		public static readonly DependencyProperty PlacementTargetProperty = DPM.Register<FrameworkElement, DropCompass>
			("PlacementTarget", c => c.OnPlacementTargetPropertyChanged);

		private static readonly DependencyPropertyKey ActualPlacementTargetPropertyKey = DPM.RegisterReadOnly<FrameworkElement, DropCompass>
			("ActualPlacementTarget");

		public static readonly DependencyProperty ActualPlacementTargetProperty = ActualPlacementTargetPropertyKey.DependencyProperty;

		public static readonly DependencyProperty IsDocumentBandEnabledProperty = DPM.Register<bool, DropCompass>
			("IsDocumentBandEnabled", true);

		private static readonly List<string> DropGuideNames = new()
		{
			"AutoHideLeftGuide",
			"DockLeftGuide",
			"AutoHideTopGuide",
			"DockTopGuide",
			"AutoHideRightGuide",
			"DockRightGuide",
			"AutoHideBottomGuide",
			"DockBottomGuide",
			"SplitLeftGuide",
			"SplitDocumentLeftGuide",
			"SplitTopGuide",
			"SplitDocumentTopGuide",
			"SplitRightGuide",
			"SplitDocumentRightGuide",
			"SplitBottomGuide",
			"SplitDocumentBottomGuide",
			"TabCenterGuide",
			"TabLeftGuide",
			"TabTopGuide",
			"TabRightGuide",
			"TabBottomGuide"
		};

		private List<DropGuide> _dropGuides;

		static DropCompass()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropCompass>();
		}

		protected DropCompass()
		{
			this.OverrideStyleKey<DropCompass>();
		}

		public FrameworkElement ActualPlacementTarget
		{
			get => (FrameworkElement) GetValue(ActualPlacementTargetProperty);
			private set => this.SetReadOnlyValue(ActualPlacementTargetPropertyKey, value);
		}

		public DropGuideAction AllowedActions
		{
			get => (DropGuideAction) GetValue(AllowedActionsProperty);
			set => SetValue(AllowedActionsProperty, value);
		}

		public IEnumerable<DropGuide> DropGuides => _dropGuides.AsEnumerable();

		public bool IsDocumentBandEnabled
		{
			get => (bool) GetValue(IsDocumentBandEnabledProperty);
			set => SetValue(IsDocumentBandEnabledProperty, value.Box());
		}

		public FrameworkElement PlacementTarget
		{
			get => (FrameworkElement) GetValue(PlacementTargetProperty);
			set => SetValue(PlacementTargetProperty, value);
		}

		private void AttachEvent(FrameworkElement newTarget)
		{
			newTarget.LayoutUpdated += OnPlacementTargetLayoutUpdated;
		}

		private void DetachEvent(FrameworkElement oldTarget)
		{
			oldTarget.LayoutUpdated -= OnPlacementTargetLayoutUpdated;
		}

		private void OnAllowedActionsPropertyChanged(DropGuideAction oldActions, DropGuideAction newActions)
		{
			UpdateAllowedActions();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (_dropGuides != null)
				foreach (var dropGuide in _dropGuides)
					dropGuide.Compass = null;

			_dropGuides = DropGuideNames.Select(GetTemplateChild).OfType<DropGuide>().SkipNull().ToList();

			foreach (var dropGuide in _dropGuides)
				dropGuide.Compass = this;

			UpdateAllowedActions();
		}

		protected virtual void OnPlacementTargetChanged()
		{
			IsDocumentBandEnabled = PlacementTarget.Return(p => p.GetVisualAncestors().Any(t => t is DocumentLayout));
		}

		private void OnPlacementTargetLayoutUpdated(object sender, EventArgs e)
		{
		}

		private void OnPlacementTargetPropertyChanged(FrameworkElement oldTarget, FrameworkElement newTarget)
		{
			if (oldTarget != null)
				DetachEvent(oldTarget);

			if (newTarget != null)
				AttachEvent(newTarget);

			OnPlacementTargetChanged();
			UpdatePlacement();
		}

		private void UpdateAllowedActions()
		{
			if (_dropGuides == null)
				return;

			foreach (var dropGuide in _dropGuides)
				dropGuide.IsAllowed = (AllowedActions & dropGuide.Action) != 0;
		}

		private void UpdatePlacement()
		{
			ActualPlacementTarget = PlacementTarget;
		}
	}
}