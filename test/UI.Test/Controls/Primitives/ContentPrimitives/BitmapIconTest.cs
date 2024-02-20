// <copyright file="BitmapIconTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Threading;
using System.Windows;
using System.Windows.Media;
using NUnit.Framework;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Test.Controls.Primitives.ContentPrimitives
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public class BitmapIconTest : UITestBase<App>
	{
		[Test]
		public void BitmapIconShouldNotAutoStretch()
		{
			var iconSize = new Size(16, 16);
			var icon = new BitmapIcon
			{
				Source = new DrawingImage
				{
					Drawing = new GeometryDrawing
					{
						Brush = Brushes.Transparent,
						Geometry = new RectangleGeometry(new Rect(iconSize))
					}
				}
			};

			RenderElement(new Button
			{
				Icon = icon
			});

			Assert.AreEqual(iconSize, icon.RenderSize);
		}
	}
}