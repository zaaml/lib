// <copyright file="InteractivityTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Test.Interactivity
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public class InteractivityTest
	{
		private ResourceDictionary _interactivityResourceDictionary;

		private static TestTheme Theme => TestTheme.Instance;

		[SetUp]
		protected void Init()
		{
			var interactivityTestUri = Assembly.GetExecutingAssembly().GetResourceUri("/Interactivity/InteractivityTest.xaml");

			_interactivityResourceDictionary = (ResourceDictionary)Application.LoadComponent(interactivityTestUri);
		}

		[Test(Description = "Test Resolvers")]
		public void TestResolvers()
		{
			ThemeManager.ApplicationTheme = Theme;

			var testContainer = new TestContainerControl
			{
				Template = (ControlTemplate)_interactivityResourceDictionary["TestResolvers"]
			};

			testContainer.ApplyTemplate();

			// Open trigger
			testContainer.ChildControl2.Input1 = 10;

			testContainer.ChildControl1 = new TestChildControl
			{
				Name = "ChildControl1"
			};

			var result = testContainer.ChildControl1.Output1;

			result = 0;
		}

		[Test(Description = "Test Simple Setters")]
		public void TestSimpleSetters()
		{
			ThemeManager.ApplicationTheme = Theme;

			var testContainer = new TestContainerControl
			{
				Input4 = 4,
				Template = (ControlTemplate)_interactivityResourceDictionary["TestSimpleSetters"]
			};

			testContainer.SetExpandoValue("ExpandoInput1", 1);
			testContainer.SetExpandoValue("ExpandoInput2", 2);
			testContainer.SetExpandoValue("ExpandoInput3", 3);
			testContainer.SetExpandoValue("ExpandoInput4", 4);

			testContainer.ApplyTemplate();

			VisualStateManager.GoToState(testContainer, "State1", false);
			VisualStateManager.GoToState(testContainer, "State3", false);

			Assert.That(testContainer.ChildControl1.Output1, Is.EqualTo(1));
			Assert.That(testContainer.ChildControl1.Output2, Is.EqualTo(2));
			Assert.That(testContainer.ChildControl1.Output3, Is.EqualTo(3));
			Assert.That(testContainer.ChildControl1.Output4, Is.EqualTo(4));

			Assert.That(testContainer.ChildControl2.Output1, Is.EqualTo(1));
			Assert.That(testContainer.ChildControl2.Output2, Is.EqualTo(2));
			Assert.That(testContainer.ChildControl2.Output3, Is.EqualTo(3));
			Assert.That(testContainer.ChildControl2.Output4, Is.EqualTo(4));

			Assert.That(testContainer.ChildControl3.Output1, Is.EqualTo(1));
			Assert.That(testContainer.ChildControl3.Output2, Is.EqualTo(2));
			Assert.That(testContainer.ChildControl3.Output3, Is.EqualTo(3));

			Assert.That(testContainer.ChildControl3.GetExpandoValue("SelfExpando4"), Is.EqualTo(4));
			Assert.That(testContainer.ChildControl3.Output4, Is.EqualTo(4));

			Assert.That(testContainer.ChildControl4.Output1, Is.EqualTo(1));
			Assert.That(testContainer.ChildControl4.Output2, Is.EqualTo(3));
			VisualStateManager.GoToState(testContainer, "State2", false);
			Assert.That(testContainer.ChildControl4.Output1, Is.EqualTo(2));
			Assert.That(testContainer.ChildControl4.Output2, Is.EqualTo(3));
			VisualStateManager.GoToState(testContainer, "State4", false);
			Assert.That(testContainer.ChildControl4.Output2, Is.EqualTo(3));
			VisualStateManager.GoToState(testContainer, "State1", false);
			Assert.That(testContainer.ChildControl4.Output2, Is.EqualTo(4));
			VisualStateManager.GoToState(testContainer, "State3", false);
			Assert.That(testContainer.ChildControl4.Output2, Is.EqualTo(3));
		}

		[Test(Description = "Test Triggers")]
		public void TestTriggers()
		{
			ThemeManager.ApplicationTheme = Theme;

			var testContainer = new TestContainerControl
			{
				Input4 = 4,
				Template = (ControlTemplate)_interactivityResourceDictionary["TestTriggers"]
			};

			testContainer.SetExpandoValue("ExpandoInput1", 1);
			testContainer.SetExpandoValue("ExpandoInput2", 2);
			testContainer.SetExpandoValue("ExpandoInput3", 3);
			testContainer.SetExpandoValue("ExpandoInput4", 4);

			testContainer.ApplyTemplate();

			// Trigger
			Assert.That(testContainer.ChildControl1.Output1, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output2, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output3, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output4, Is.EqualTo(0));

			testContainer.ChildControl1.Input1 = 10;
			Assert.That(testContainer.ChildControl1.Output1, Is.EqualTo(10));

			testContainer.ChildControl1.Input2 = 20;
			Assert.That(testContainer.ChildControl1.Output2, Is.EqualTo(20));

			testContainer.ChildControl1.Input3 = 30;
			Assert.That(testContainer.ChildControl1.Output3, Is.EqualTo(30));

			Extension.GetTriggers(testContainer.ChildControl1).Clear();
			Assert.That(testContainer.ChildControl1.Output1, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output2, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output3, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output4, Is.EqualTo(0));

			// DataTrigger
			testContainer.Input1 = -1;

			Assert.That(testContainer.ChildControl2.Output1, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl2.Output2, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl2.Output3, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl2.Output4, Is.EqualTo(0));

			testContainer.Input1 = 10;
			Assert.That(testContainer.ChildControl2.Output1, Is.EqualTo(10));

			testContainer.ChildControl2.Input2 = 10;
			Assert.That(testContainer.ChildControl2.Output2, Is.EqualTo(20));

			Extension.GetTriggers(testContainer.ChildControl2).Clear();
			Assert.That(testContainer.ChildControl1.Output1, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output2, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output3, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl1.Output4, Is.EqualTo(0));

			// MultiDataTrigger

			Assert.That(testContainer.ChildControl3.Output1, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl3.Output2, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl3.Output3, Is.EqualTo(0));
			Assert.That(testContainer.ChildControl3.Output4, Is.EqualTo(0));

			testContainer.ChildControl3.Input1 = 10;
			Assert.That(testContainer.ChildControl3.Output1, Is.EqualTo(0));
			testContainer.ChildControl3.Input2 = 20;
			Assert.That(testContainer.ChildControl3.Output1, Is.EqualTo(10));
			testContainer.ChildControl3.Input2 = 30;
			Assert.That(testContainer.ChildControl3.Output1, Is.EqualTo(0));
			testContainer.ChildControl3.Input3 = 30;
			Assert.That(testContainer.ChildControl3.Output1, Is.EqualTo(10));
		}
	}
}