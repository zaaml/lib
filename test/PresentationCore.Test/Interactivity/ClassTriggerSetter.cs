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

			Assert.AreEqual(0, control.Input1);
			Assert.AreEqual(0, control.Input2);

			Extension.AddClass(control, "in1");

			Assert.AreEqual(1, control.Input1);
			Assert.AreEqual(0, control.Input2);

			Extension.AddClass(control, "in2");

			Assert.AreEqual(1, control.Input1);
			Assert.AreEqual(2, control.Input2);

			Extension.RemoveClass(control, "in1");

			Assert.AreEqual(0, control.Input1);
			Assert.AreEqual(2, control.Input2);
		}
	}
}