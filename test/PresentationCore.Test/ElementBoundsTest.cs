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

			Rect a1 = new Rect(100, 100, 200, 200);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a1));

			var applyTransform = elementBounds;
			var applyTranslate = elementBounds;
			var applyScale = elementBounds;


			// ApplyTransform
			applyTransform.ApplyTransform();

			Rect a2 = new Rect(100, 100, 200, 200);
			Assert.That(applyTransform.TransformedBounds, Is.EqualTo(a2));
			Rect a3 = new Rect(100, 100, 200, 200);
			Assert.That(applyTransform.Bounds, Is.EqualTo(a3));

			// ApplyTranslate
			applyTranslate.ApplyTranslate();

			Rect a4 = new Rect(100, 100, 200, 200);
			Assert.That(applyTranslate.TransformedBounds, Is.EqualTo(a4));

			// ApplyScale
			applyScale.ApplyTransform();

			Rect a5 = new Rect(100, 100, 200, 200);
			Assert.That(applyScale.TransformedBounds, Is.EqualTo(a5));
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
			Assert.That(1.0, Is.EqualTo(elementBounds.RelativeCenterX));
			Assert.That(1.0, Is.EqualTo(elementBounds.RelativeCenterY));

			var transformedBounds = elementBounds.TransformedBounds;
			var expectedBounds = new Rect(-100, -100, 200, 200);

			// Check bounds
			Assert.That(expectedBounds, Is.EqualTo(transformedBounds));

			// Check transformed center
			Assert.That(transformedBounds.GetBottomRight(), Is.EqualTo(elementBounds.TransformedCenter));

			elementBounds.Bounds = new Rect();

			elementBounds.TransformedBounds = transformedBounds;

			// Check transformation
			Assert.That(elementBounds.Bounds, Is.EqualTo(originalBounds));
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
			Assert.That(transformedBounds.Right, Is.EqualTo(elementBounds.CenterX));
			Assert.That(transformedBounds.Bottom, Is.EqualTo(elementBounds.CenterY));

			// Check local center
			Assert.That(elementBounds.Bounds.Width, Is.EqualTo(elementBounds.LocalCenterX));
			Assert.That(elementBounds.Bounds.Height, Is.EqualTo(elementBounds.LocalCenterY));

			// Change local center
			elementBounds.LocalCenterX = 50;
			elementBounds.LocalCenterY = 50;

			// Check relative center
			Assert.That(elementBounds.RelativeCenterX, Is.EqualTo(0.5));
			Assert.That(elementBounds.RelativeCenterY, Is.EqualTo(0.5));
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

			Assert.That(elementBounds.Bounds, Is.EqualTo(originalBounds));
			Rect a1 = new Rect(150, 150, 600, 600);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a1));

			elementBounds.ChangeRelativeCenterPreserveTransform(0.5, 0.5);

			Assert.That(elementBounds.Bounds, Is.EqualTo(originalBounds));
			Rect a2 = new Rect(150, 150, 600, 600);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a2));
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

			Assert.That(elementBounds.Bounds, Is.EqualTo(originalBounds));
			Assert.That(originalCenter, Is.EqualTo(elementBounds.Center));
			Rect a1 = new Rect(-400, -400, 800, 800);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a1));

			elementBounds.ChangeTranslatePreserveTransform(-200, -200);

			Rect a2 = new Rect(-400, -400, 800, 800);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a2));

			elementBounds.Scale(4, 4, 400, 400);

			Assert.That(elementBounds.Bounds, Is.EqualTo(originalBounds));
			Rect a3 = new Rect(-1200, -1200, 1600, 1600);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a3));

			elementBounds.ChangeRelativeCenterPreserveTransform(1, 1);

			Rect a4 = new Rect(-1200, -1200, 1600, 1600);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a4));
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

			Assert.That(elementBounds.Bounds, Is.EqualTo(originalBounds));
			Rect a1 = new Rect(1400, 1400, 1600, 1600);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a1));
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

			Assert.That(elementBounds.Bounds, Is.EqualTo(originalBounds));
			Rect a1 = new Rect(0, 0, 800, 800);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a1));

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
			Assert.That(elementBounds.TransformedBounds.GetCenter(), Is.EqualTo(elementBounds.TransformedCenter));

			elementBounds.TransformedCenterX = elementBounds.TransformedBounds.Right;
			elementBounds.TransformedCenterY = elementBounds.TransformedBounds.Bottom;

			// Check relative center
			Assert.That(elementBounds.RelativeCenterX, Is.EqualTo(1.0));
			Assert.That(elementBounds.RelativeCenterY, Is.EqualTo(1.0));

			elementBounds.RelativeCenter = new Point(0, 0);
			elementBounds.TransformedLocalCenter = new Point(100, 100);

			// Check relative center
			Point a1 = new Point(0.5, 0.5);
			Assert.That(elementBounds.RelativeCenter, Is.EqualTo(a1));
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

			Rect a1 = new Rect(100, 100, 200, 200);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a1));

			elementBounds.Center = elementBounds.Translate;

			Rect a2 = new Rect(0, 0, 200, 200);
			Assert.That(elementBounds.TransformedBounds, Is.EqualTo(a2));
		}
	}
}