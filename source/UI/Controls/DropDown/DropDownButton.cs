// <copyright file="DropDownButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.DropDown
{
	public class DropDownButton : DropDownButtonBase
	{
		public static readonly DependencyProperty DropDownGlyphDockProperty = DPM.Register<Dock?, DropDownButton>
			("DropDownGlyphDock", Dock.Right);

		public static readonly DependencyProperty PlacementTargetProperty = DPM.Register<FrameworkElement, DropDownButton>
			("PlacementTarget", d => d.OnPlacementTargetChanged);

		public static readonly DependencyProperty ShowDropDownGlyphProperty = DPM.Register<bool, DropDownButton>
			("ShowDropDownGlyph", true);

		static DropDownButton()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownButton>();
		}

		public DropDownButton()
		{
			this.OverrideStyleKey<DropDownButton>();
		}

		public Dock? DropDownGlyphDock
		{
			get => (Dock?) GetValue(DropDownGlyphDockProperty);
			set => SetValue(DropDownGlyphDockProperty, value);
		}

		public FrameworkElement PlacementTarget
		{
			get => (FrameworkElement) GetValue(PlacementTargetProperty);
			set => SetValue(PlacementTargetProperty, value);
		}

		public bool ShowDropDownGlyph
		{
			get => (bool) GetValue(ShowDropDownGlyphProperty);
			set => SetValue(ShowDropDownGlyphProperty, value);
		}

		protected override void OnClick()
		{
			base.OnClick();

			if (PopupControl == null) 
				return;
			
			if (IsDropDownOpen)
				CloseDropDown();
			else
				OpenDropDown();
		}

		private void OnPlacementTargetChanged()
		{
			PlacementTargetCore = PlacementTarget ?? this;
		}
	}
}