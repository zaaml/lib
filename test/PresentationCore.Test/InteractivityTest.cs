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

namespace Zaaml.PresentationCore.Test
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
			var interactivityTestUri = Assembly.GetExecutingAssembly().GetResourceUri("/InteractivityTest.xaml");

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

			Assert.AreEqual(1, testContainer.ChildControl1.Output1, "Constant value");
			Assert.AreEqual(2, testContainer.ChildControl1.Output2, "StaticResource Value");
			Assert.AreEqual(3, testContainer.ChildControl1.Output3, "Binding to StaticResource Value");
			Assert.AreEqual(4, testContainer.ChildControl1.Output4, "TemplateBinding Value");

			Assert.AreEqual(1, testContainer.ChildControl2.Output1, "Constant Value To TargetName");
			Assert.AreEqual(2, testContainer.ChildControl2.Output2, "StaticResource Value To Explicit Target");
			Assert.AreEqual(3, testContainer.ChildControl2.Output3, "Groupped by TargetName: Binding to StaticResource Value");
			Assert.AreEqual(4, testContainer.ChildControl2.Output4, "Groupped by Property: TemplateBinding Value");

			Assert.AreEqual(1, testContainer.ChildControl3.Output1, "ThemeResource Value");
			Assert.AreEqual(2, testContainer.ChildControl3.Output2, "Groupped by ThemeResource ValuePathSource: ValuePath");
			Assert.AreEqual(3, testContainer.ChildControl3.Output3, "TemplateExpando");

			Assert.AreEqual(4, testContainer.ChildControl3.GetExpandoValue("SelfExpando4"), "Expando value");
			Assert.AreEqual(4, testContainer.ChildControl3.Output4, "Groupped by Property and SelfExpando ValuePathSource: ValuePath");

			Assert.AreEqual(1, testContainer.ChildControl4.Output1, "State1: ThemeResource Value");
			Assert.AreEqual(3, testContainer.ChildControl4.Output2, "State3 implicit Priority: TemplateExpando");
			VisualStateManager.GoToState(testContainer, "State2", false);
			Assert.AreEqual(2, testContainer.ChildControl4.Output1, "State2: ThemeResource ValuePathSource: ValuePath");
			Assert.AreEqual(3, testContainer.ChildControl4.Output2, "State2 Priority2: TemplateExpando");
			VisualStateManager.GoToState(testContainer, "State4", false);
			Assert.AreEqual(3, testContainer.ChildControl4.Output2, "State2 Priority2: TemplateExpando");
			VisualStateManager.GoToState(testContainer, "State1", false);
			Assert.AreEqual(4, testContainer.ChildControl4.Output2, "State4 Priority1: SelfExpando ValuePathSource: ValuePath");
			VisualStateManager.GoToState(testContainer, "State3", false);
			Assert.AreEqual(3, testContainer.ChildControl4.Output2, "State3 implicit Priority: TemplateExpando");
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
			Assert.AreEqual(0, testContainer.ChildControl1.Output1, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output2, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output3, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output4, "Default Value");

			testContainer.ChildControl1.Input1 = 10;
			Assert.AreEqual(10, testContainer.ChildControl1.Output1, "Trigger: Self Property");

			testContainer.ChildControl1.Input2 = 20;
			Assert.AreEqual(20, testContainer.ChildControl1.Output2, "Trigger: Explicit Source Property");

			testContainer.ChildControl1.Input3 = 30;
			Assert.AreEqual(30, testContainer.ChildControl1.Output3, "Trigger: Source Name Property");

			Extension.GetTriggers(testContainer.ChildControl1).Clear();
			Assert.AreEqual(0, testContainer.ChildControl1.Output1, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output2, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output3, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output4, "Default Value");

			// DataTrigger
			testContainer.Input1 = -1;

			Assert.AreEqual(0, testContainer.ChildControl2.Output1, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl2.Output2, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl2.Output3, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl2.Output4, "Default Value");

			testContainer.Input1 = 10;
			Assert.AreEqual(10, testContainer.ChildControl2.Output1, "DataTrigger: TemplatedParent Property To Constant");

			testContainer.ChildControl2.Input2 = 10;
			Assert.AreEqual(20, testContainer.ChildControl2.Output2, "DataTrigger: SelfProperty To TemplatedParent Property");

			Extension.GetTriggers(testContainer.ChildControl2).Clear();
			Assert.AreEqual(0, testContainer.ChildControl1.Output1, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output2, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output3, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl1.Output4, "Default Value");

			// MultiDataTrigger

			Assert.AreEqual(0, testContainer.ChildControl3.Output1, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl3.Output2, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl3.Output3, "Default Value");
			Assert.AreEqual(0, testContainer.ChildControl3.Output4, "Default Value");

			testContainer.ChildControl3.Input1 = 10;
			Assert.AreEqual(0, testContainer.ChildControl3.Output1, "Default Value");
			testContainer.ChildControl3.Input2 = 20;
			Assert.AreEqual(10, testContainer.ChildControl3.Output1, "And group");
			testContainer.ChildControl3.Input2 = 30;
			Assert.AreEqual(0, testContainer.ChildControl3.Output1, "Default Value");
			testContainer.ChildControl3.Input3 = 30;
			Assert.AreEqual(10, testContainer.ChildControl3.Output1, "Or modifier");
		}
	}
}