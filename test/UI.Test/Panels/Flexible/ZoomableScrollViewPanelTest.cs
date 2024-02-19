//// <copyright file="ZoomableScrollViewPanelTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
////   Copyright (c) Zaaml. All rights reserved.
//// </copyright>

//using System.Linq;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;
//using NUnit.Framework;
//using Zaaml.PresentationCore.Extensions;
//using Zaaml.PresentationCore.Theming;
//using Zaaml.Theming;
//using Zaaml.UI.Controls.ArtBoard;

//namespace Zaaml.UI.Test.Panels.Flexible
//{
//	[TestFixture, Apartment(ApartmentState.STA)]
//	internal sealed class ZoomableScrollViewPanelTest
//	{
//		#region Fields

//		private ZoomableScrollViewPanel _panel;
//		private ZoomableScrollViewControl _scrollViewControl;

//		#endregion

//		#region  Methods

//		[SetUp]
//		public void Init()
//		{
//			var app = new Application();

//			ThemeManager.ApplicationTheme = Themes.MetroUIOffice;

//			var window = new Window { Opacity = 0, AllowsTransparency = true, WindowStyle = WindowStyle.None, ShowInTaskbar = false };

//			window.Show();

//			_scrollViewControl = new ZoomableScrollViewControl
//			{
//				Child = new Border { Width = 100, Height = 100 }
//			};

//			window.Content = _scrollViewControl;

//			_scrollViewControl.ApplyTemplate();

//			_scrollViewControl.Measure(new Size(100,100));

//			_panel = _scrollViewControl.GetVisualDescendants().OfType<ZoomableScrollViewPanel>().Single();

//			_panel.ActualViewport = new Size(800, 800);

//			_panel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
//		}

//		[Test(Description = "Zoom")]
//		public void Zoom()
//		{
//			_panel.OnMouseZoom(new Point(0,0),  2);
//		}

//		#endregion
//	}
//}