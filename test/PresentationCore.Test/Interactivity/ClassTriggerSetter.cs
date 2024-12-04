// <copyright file="ClassTriggerSetter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Threading;
using NUnit.Framework;

namespace Zaaml.PresentationCore.Test.Interactivity
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	internal class ClassTriggerSetter
	{
		[Test]
		public void BasicTest()
		{
			var control = new ClassTriggerSetterTestControl().TestChildControl;

			Assert.That(control.Input1, Is.EqualTo(0));
			Assert.That(control.Input2, Is.EqualTo(0));

			Extension.AddClass(control, "in1");

			Assert.That(control.Input1, Is.EqualTo(1));
			Assert.That(control.Input2, Is.EqualTo(0));

			Extension.AddClass(control, "in2");

			Assert.That(control.Input1, Is.EqualTo(1));
			Assert.That(control.Input2, Is.EqualTo(2));

			Extension.RemoveClass(control, "in1");

			Assert.That(control.Input1, Is.EqualTo(0));
			Assert.That(control.Input2, Is.EqualTo(2));
		}
	}
}