// <copyright file="ScreenPositionWindow.xaml.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;

namespace Zaaml.UI.Test
{
	public partial class ScreenPositionWindow : Window
	{
		public ScreenPositionWindow()
		{
			InitializeComponent();
			
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			var scrPos = MouseInternal.ScreenDevicePosition.LayoutRound(RoundingMode.MidPointFromZero);
			var scrPos1 = PointToScreen(e.GetPosition(this));

			var borderBox = RedBorder.GetScreenDeviceBox();

			var topLeft = RedBorder.PointToScreen(new Point(0, 0));
			var bottomRight = RedBorder.PointToScreen(new Point(ActualWidth, ActualHeight));
			var rightBox = new Rect(topLeft, bottomRight);
			
			Info.Text = $"ScreenPosCustom: {scrPos}\nScreenPosRight: {scrPos1}\nBorderBox: {borderBox}\nRightBox: {rightBox}";
		}
	}
}