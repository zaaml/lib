// <copyright file="DropGuide.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using ZaamlContentControl = System.Windows.Controls.ContentControl;

namespace Zaaml.UI.Controls.Docking
{
	public class DropGuide : ZaamlContentControl
	{
		public static readonly DependencyProperty ActionProperty = DPM.Register<DropGuideAction, DropGuide>
			("Action");

		private static readonly DependencyPropertyKey IsAllowedPropertyKey = DPM.RegisterReadOnly<bool, DropGuide>
			("IsAllowed", d => d.OnIsAllowedPropertyChanged);

		private static readonly DependencyPropertyKey IsActivePropertyKey = DPM.RegisterReadOnly<bool, DropGuide>
			("IsActive");

		public static readonly DependencyProperty IsActiveProperty = IsActivePropertyKey.DependencyProperty;

		public static readonly DependencyProperty IsAllowedProperty = IsAllowedPropertyKey.DependencyProperty;

		static DropGuide()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropGuide>();
		}

		public DropGuide()
		{
			this.OverrideStyleKey<DropGuide>();
		}

		public DropGuideAction Action
		{
			get => (DropGuideAction) GetValue(ActionProperty);
			set => SetValue(ActionProperty, value);
		}

		internal DropCompass Compass { get; set; }

		public bool IsActive
		{
			get => (bool) GetValue(IsActiveProperty);
			internal set => this.SetReadOnlyValue(IsActivePropertyKey, value);
		}

		public bool IsAllowed
		{
			get => (bool) GetValue(IsAllowedProperty);
			internal set => this.SetReadOnlyValue(IsAllowedPropertyKey, value);
		}

		public Dock? Side => GetGuideSide(Action);

		public static DropGuideActionType GetActionType(DropGuideAction dropGuideAction)
		{
			switch (dropGuideAction)
			{
				case DropGuideAction.SplitLeft:
				case DropGuideAction.SplitTop:
				case DropGuideAction.SplitRight:
				case DropGuideAction.SplitBottom:
					return DropGuideActionType.Split;

				case DropGuideAction.SplitDocumentLeft:
				case DropGuideAction.SplitDocumentTop:
				case DropGuideAction.SplitDocumentRight:
				case DropGuideAction.SplitDocumentBottom:
					return DropGuideActionType.SplitDocument;

				case DropGuideAction.AutoHideLeft:
				case DropGuideAction.AutoHideTop:
				case DropGuideAction.AutoHideRight:
				case DropGuideAction.AutoHideBottom:
					return DropGuideActionType.AutoHide;

				case DropGuideAction.TabLeft:
				case DropGuideAction.TabTop:
				case DropGuideAction.TabRight:
				case DropGuideAction.TabBottom:
				case DropGuideAction.TabCenter:
					return DropGuideActionType.Tab;

				case DropGuideAction.DockLeft:
				case DropGuideAction.DockTop:
				case DropGuideAction.DockRight:
				case DropGuideAction.DockBottom:
					return DropGuideActionType.Dock;
			}

			return DropGuideActionType.Undefined;
		}

		public static Dock? GetGuideSide(DropGuideAction dropGuideAction)
		{
			switch (dropGuideAction)
			{
				case DropGuideAction.SplitLeft:
				case DropGuideAction.SplitDocumentLeft:
				case DropGuideAction.AutoHideLeft:
				case DropGuideAction.TabLeft:
				case DropGuideAction.DockLeft:
					return Dock.Left;

				case DropGuideAction.SplitTop:
				case DropGuideAction.SplitDocumentTop:
				case DropGuideAction.AutoHideTop:
				case DropGuideAction.TabTop:
				case DropGuideAction.DockTop:
					return Dock.Top;

				case DropGuideAction.SplitRight:
				case DropGuideAction.SplitDocumentRight:
				case DropGuideAction.AutoHideRight:
				case DropGuideAction.TabRight:
				case DropGuideAction.DockRight:
					return Dock.Right;

				case DropGuideAction.SplitBottom:
				case DropGuideAction.SplitDocumentBottom:
				case DropGuideAction.AutoHideBottom:
				case DropGuideAction.TabBottom:
				case DropGuideAction.DockBottom:
					return Dock.Bottom;

				case DropGuideAction.TabCenter:
					return null;
			}

			return null;
		}

		private void OnIsAllowedPropertyChanged()
		{
			InvalidateVisual();
		}
	}
}