// <copyright file="SetterMutabilityTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Threading;
using System.Windows.Markup;
using NUnit.Framework;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Test
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public class SetterMutabilityTest
	{
		private static TestTheme Theme => TestTheme.Instance;

		[Test(Description = "Change Priority")]
		public void ChangePriority()
		{
			ThemeManager.ApplicationTheme = Theme;

			var testControl = new TestChildControl();

			var setter1 = new Setter { Property = "Output1", Value = 10 };
			var setter2 = new Setter { Property = "Output1", Value = 20 };

			var trigger = new Trigger { Property = "Input1", Value = 10, Setters = { setter1, setter2 } };

			Extension.GetTriggers(testControl).Add(trigger);

			Assert.AreEqual(0, testControl.Output1, "Initial state");

			// Open trigger
			testControl.Input1 = 10;
			Assert.AreEqual(20, testControl.Output1, "Second setter has higher priority (Order)");

			// Make first setter topmost
			setter1.Priority = 1;
			Assert.AreEqual(10, testControl.Output1, "First setter has higher priority (Explicit)");

			// Make second setter topmost
			setter2.Priority = 2;
			Assert.AreEqual(20, testControl.Output1, "Second setter has higher priority (Explicit)");

			// Remove second setter
			trigger.Setters.Remove(setter2);
			Assert.AreEqual(10, testControl.Output1, "First setter is single (second removed)");

			// Remove first setter
			trigger.Setters.Remove(setter1);
			Assert.AreEqual(testControl.Output1, 0, "Initial state (all setters removed)");

			// Add second setter
			trigger.Setters.Add(setter2);
			Assert.AreEqual(20, testControl.Output1, "Second setter has higher priority (single)");

			// Make first setter topmost and add to trigger
			setter1.Priority = 3;
			trigger.Setters.Add(setter1);
			Assert.AreEqual(10, testControl.Output1, "First setter has higher priority (Explicit)");
		}

		[Test(Description = "Change Property")]
		public void ChangeProperty()
		{
			ThemeManager.ApplicationTheme = Theme;

			var testControl = new TestChildControl();

			var setter1 = new Setter { Property = "Output1", Value = 10 };
			var setter2 = new Setter { Property = "Output2", Value = 20 };

			var trigger = new Trigger { Property = "Input1", Value = 10, Setters = { setter1, setter2 } };

			Extension.GetTriggers(testControl).Add(trigger);

			Assert.AreEqual(0, testControl.Output1, "Initial state");
			Assert.AreEqual(0, testControl.Output2, "Initial state");

			// Open trigger
			testControl.Input1 = 10;

			Assert.AreEqual(10, testControl.Output1, "Output1 Modified with first setter");
			Assert.AreEqual(20, testControl.Output2, "Output2 Modified with second setter");

			// Change setter1 property
			setter1.Property = "Output3";
			Assert.AreEqual(0, testControl.Output1, "Output1 Back to initial state");
			Assert.AreEqual(10, testControl.Output3, "Output3 modified with first setter");

			// Change setter1 property to Explicit DependencyProperty
			setter1.Property = TestChildControl.Output4Property;
			Assert.AreEqual(0, testControl.Output3, "Output3 Back to initial state");
			Assert.AreEqual(10, testControl.Output4, "Output4 modified with first setter");

			// Change setter2 property to implicit Expando property
			setter2.ExpandoProperty = "Expando1";
			Assert.AreEqual(0, testControl.Output2, "Output2 Back to initial state");
			Assert.AreEqual(20, testControl.GetExpandoValue("Expando1"), "Expando1 modified with second setter");
		}

		[Test(Description = "Change Target")]
		public void ChangeTarget()
		{
			ThemeManager.ApplicationTheme = Theme;

			var testContainer = new TestContainerControl();

			var nameScope = new NameScope() as INameScope;


			var testControl1 = new TestChildControl { Name = "ChildControl1" };
			var testControl2 = new TestChildControl { Name = "ChildControl2" };

			testContainer.ChildControl1 = testControl1;
			testContainer.ChildControl2 = testControl2;

			nameScope.RegisterName(testControl1.Name, testControl1);
			nameScope.RegisterName(testControl2.Name, testControl2);

			NameScope.SetNameScope(testContainer, nameScope);

			var setter1 = new Setter { Target = testControl1, Property = "Output1", Value = 10 };
			var setter2 = new Setter { Target = testControl2, Property = "Output2", Value = 20 };

			var trigger = new Trigger { Property = "Input1", Value = 10, Setters = { setter1, setter2 } };

			Extension.GetTriggers(testContainer).Add(trigger);

			// Open trigger
			testContainer.Input1 = 10;

			Assert.AreEqual(10, testControl1.Output1, "Output1 changed by first setter");
			Assert.AreEqual(20, testControl2.Output2, "Output2 changed by second setter");

			// Change first setter target to null
			setter1.Target = null;
			Assert.AreEqual(0, testControl1.Output1, "Output1 back to initial state");

			// Change second setter target to first control
			setter2.Target = testControl1;
			Assert.AreEqual(20, testControl1.Output2, "Output2 changed by second setter");

			// Change first setter target name to second control
			setter1.TargetName = testControl2.Name;
			Assert.AreEqual(10, testControl2.Output1, "Output1 changed by first setter");
		}

		[Test(Description = "Change Transition")]
		public void ChangeTransition()
		{
		}

		[Test(Description = "Change Value")]
		public void ChangeValue()
		{
			ThemeManager.ApplicationTheme = Theme;

			var testControl = new TestChildControl();

			var setter = new Setter { Property = "Output1", Value = 10 };

			var trigger = new Trigger { Property = "Input1", Value = 10, Setters = { setter } };

			Extension.GetTriggers(testControl).Add(trigger);

			// Initial state
			Assert.AreEqual(0, testControl.Output1, "Initial state");

			// Open trigger
			testControl.Input1 = 10;

			Assert.AreEqual(10, testControl.Output1, "Output1 changed by setter");

			// Change value
			setter.Value = 20;

			Assert.AreEqual(20, testControl.Output1, "Setter value changed to 20");
		}

		[Test(Description = "Change ValuePath")]
		public void ChangeValuePath()
		{
		}

		[Test(Description = "Change ValuePathSource")]
		public void ChangeValuePathSource()
		{
		}

		[Test(Description = "Change VisualState")]
		public void ChangeVisualState()
		{
		}

		[SetUp]
		public void Init()
		{
		}
	}
}