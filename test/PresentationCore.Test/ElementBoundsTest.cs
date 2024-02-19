// <copyright file="ElementBoundsTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using NUnit.Framework;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Test
{
	[TestFixture]
	public class ElementBoundsTest
	{
		[Test(Description = "TestApply")]
		public void TestApply()
		{
			var originalBounds = new Rect(0, 0, 100, 100);

			var elementBounds = new ElementBounds
			{
				Bounds = originalBounds,
				TranslateX = 100,
				TranslateY = 100,
				CenterX = 0,
				CenterY = 0,
				ScaleX = 2,
				ScaleY = 2
			};

			Assert.AreEqual(new Rect(100, 100, 200, 200), elementBounds.TransformedBounds);

			var applyTransform = elementBounds;
			var applyTranslate = elementBounds;
			var applyScale = elementBounds;


			// ApplyTransform
			applyTransform.ApplyTransform();

			Assert.AreEqual(new Rect(100, 100, 200, 200), applyTransform.TransformedBounds);
			Assert.AreEqual(new Rect(100, 100, 200, 200), applyTransform.Bounds);

			// ApplyTranslate
			applyTranslate.ApplyTranslate();

			Assert.AreEqual(new Rect(100, 100, 200, 200), applyTranslate.TransformedBounds);

			// ApplyScale
			applyScale.ApplyTransform();

			Assert.AreEqual(new Rect(100, 100, 200, 200), applyScale.TransformedBounds);
		}

		[Test(Description = "TestBounds")]
		public void TestBounds()
		{
			var originalBounds = new Rect(0, 0, 100, 100);

			var elementBounds = new ElementBounds
			{
				Bounds = originalBounds,
				CenterX = 100,
				CenterY = 100,
				ScaleX = 2,
				ScaleY = 2
			};

			// Check relative center
			Assert.AreEqual(elementBounds.RelativeCenterX, 1.0);
			Assert.AreEqual(elementBounds.RelativeCenterY, 1.0);

			var transformedBounds = elementBounds.TransformedBounds;
			var expectedBounds = new Rect(-100, -100, 200, 200);

			// Check bounds
			Assert.AreEqual(transformedBounds, expectedBounds);

			// Check transformed center
			Assert.AreEqual(elementBounds.TransformedCenter, transformedBounds.GetBottomRight());

			elementBounds.Bounds = new Rect();

			elementBounds.TransformedBounds = transformedBounds;

			// Check transformation
			Assert.AreEqual(originalBounds, elementBounds.Bounds);
		}

		[Test(Description = "TestCenter")]
		public void TestCenter()
		{
			var elementBounds = new ElementBounds
			{
				Bounds = new Rect(100, 100, 100, 100),
				ScaleX = 2,
				ScaleY = 2,
				RelativeCenterX = 1.0,
				RelativeCenterY = 1.0
			};

			var transformedBounds = elementBounds.TransformedBounds;

			// Check center
			Assert.AreEqual(elementBounds.CenterX, transformedBounds.Right);
			Assert.AreEqual(elementBounds.CenterY, transformedBounds.Bottom);

			// Check local center
			Assert.AreEqual(elementBounds.LocalCenterX, elementBounds.Bounds.Width);
			Assert.AreEqual(elementBounds.LocalCenterY, elementBounds.Bounds.Height);

			// Change local center
			elementBounds.LocalCenterX = 50;
			elementBounds.LocalCenterY = 50;

			// Check relative center
			Assert.AreEqual(0.5, elementBounds.RelativeCenterX);
			Assert.AreEqual(0.5, elementBounds.RelativeCenterY);
		}

		[Test(Description = "TestCenterScale")]
		public void TestCenterScale()
		{
			var originalBounds = new Rect(200, 200, 400, 400);

			var elementBounds = new ElementBounds
			{
				ScaleX = 1,
				ScaleY = 1,
				Bounds = originalBounds,
				RelativeCenter = new Point(0.5, 0.5)
			};

			elementBounds.Scale(1.5, 1.5, 300, 300);

			Assert.AreEqual(originalBounds, elementBounds.Bounds);
			Assert.AreEqual(new Rect(150, 150, 600, 600), elementBounds.TransformedBounds);

			elementBounds.ChangeRelativeCenterPreserveTransform(0.5, 0.5);

			Assert.AreEqual(originalBounds, elementBounds.Bounds);
			Assert.AreEqual(new Rect(150, 150, 600, 600), elementBounds.TransformedBounds);
		}


		[Test(Description = "TestScale")]
		public void TestScale()
		{
			var originalBounds = new Rect(0, 0, 400, 400);
			var originalCenter = new Point(0, 0);

			var elementBounds = new ElementBounds
			{
				Center = originalCenter,
				ScaleX = 1,
				ScaleY = 1,
				Bounds = originalBounds
			};

			elementBounds.Scale(2, 2, 400, 400);

			Assert.AreEqual(originalBounds, elementBounds.Bounds);
			Assert.AreEqual(elementBounds.Center, originalCenter);
			Assert.AreEqual(new Rect(-400, -400, 800, 800), elementBounds.TransformedBounds);

			elementBounds.ChangeTranslatePreserveTransform(-200, -200);

			Assert.AreEqual(new Rect(-400, -400, 800, 800), elementBounds.TransformedBounds);

			elementBounds.Scale(4, 4, 400, 400);

			Assert.AreEqual(originalBounds, elementBounds.Bounds);
			Assert.AreEqual(new Rect(-1200, -1200, 1600, 1600), elementBounds.TransformedBounds);

			elementBounds.ChangeRelativeCenterPreserveTransform(1, 1);

			Assert.AreEqual(new Rect(-1200, -1200, 1600, 1600), elementBounds.TransformedBounds);
		}

		[Test(Description = "TestScale2")]
		public void TestScale2()
		{
			var originalBounds = new Rect(300, 300, 200, 200);

			var elementBounds = new ElementBounds
			{
				CenterX = 100,
				CenterY = 100,
				ScaleX = 4,
				ScaleY = 4,
				Bounds = originalBounds
			};

			elementBounds.Scale(8, 8, 400, 400);

			Assert.AreEqual(originalBounds, elementBounds.Bounds);
			Assert.AreEqual(new Rect(1400, 1400, 1600, 1600), elementBounds.TransformedBounds);
		}

		[Test(Description = "TestScalePreserve")]
		public void TestScalePreserve()
		{
			var originalBounds = new Rect(100, 100, 400, 400);

			var elementBounds = new ElementBounds
			{
				CenterX = 100,
				CenterY = 100,
				ScaleX = 1,
				ScaleY = 1,
				Bounds = originalBounds
			};

			elementBounds.Scale(2, 2, 200, 200);

			Assert.AreEqual(originalBounds, elementBounds.Bounds);
			Assert.AreEqual(new Rect(0, 0, 800, 800), elementBounds.TransformedBounds);

			elementBounds.Scale(4, 4, 300, 200);
		}

		[Test(Description = "TestTransformedCenter")]
		public void TestTransformedCenter()
		{
			var originalBounds = new Rect(0, 0, 100, 100);

			var elementBounds = new ElementBounds
			{
				Bounds = originalBounds,
				CenterX = 50,
				CenterY = 50,
				ScaleX = 2,
				ScaleY = 2
			};

			// Check transformed center
			Assert.AreEqual(elementBounds.TransformedCenter, elementBounds.TransformedBounds.GetCenter());

			elementBounds.TransformedCenterX = elementBounds.TransformedBounds.Right;
			elementBounds.TransformedCenterY = elementBounds.TransformedBounds.Bottom;

			// Check relative center
			Assert.AreEqual(1.0, elementBounds.RelativeCenterX);
			Assert.AreEqual(1.0, elementBounds.RelativeCenterY);

			elementBounds.RelativeCenter = new Point(0, 0);
			elementBounds.TransformedLocalCenter = new Point(100, 100);

			// Check relative center
			Assert.AreEqual(new Point(0.5, 0.5), elementBounds.RelativeCenter);
		}

		[Test(Description = "TestTranslate")]
		public void TestTranslate()
		{
			var originalBounds = new Rect(0, 0, 100, 100);

			var elementBounds = new ElementBounds
			{
				Bounds = originalBounds,
				TranslateX = 100,
				TranslateY = 100,
				CenterX = 0,
				CenterY = 0,
				ScaleX = 2,
				ScaleY = 2
			};

			Assert.AreEqual(new Rect(100, 100, 200, 200), elementBounds.TransformedBounds);

			elementBounds.Center = elementBounds.Translate;

			Assert.AreEqual(new Rect(0, 0, 200, 200), elementBounds.TransformedBounds);
		}
	}
}