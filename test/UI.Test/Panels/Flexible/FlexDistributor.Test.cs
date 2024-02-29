// <copyright file="FlexDistributor.Test.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using NUnit.Framework;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Test.Panels.Flexible
{
	[TestFixture]
	public class FlexDistributor
	{
		private FlexElementCollection _elements;

		[Test(Description = "Equalize Expand")]
		public void EqualizeExpand()
		{
			var distributor = UI.Panels.Flexible.FlexDistributor.Equalizer;

			const int initialTarget = 50;

			for (var i = 0; i < 300; i++)
			{
				using (var elements = GetElements())
				{
					var target = initialTarget + i * 0.1;
					elements.Value.UseLayoutRounding = true;
					distributor.Distribute(elements.Value, target);

					Assert.AreEqual(target.Truncate(), elements.Value.Actual, $"Expand Failed: target={target}");
				}
			}
		}

		[Test(Description = "Equalize Shrink")]
		public void EqualizeShrink()
		{
			var distributor = UI.Panels.Flexible.FlexDistributor.Equalizer;

			const int initialTarget = 50;

			for (var i = 0; i < 300; i++)
			{
				using (var elements = GetElements())
				{
					var target = initialTarget - i * 0.1;
					elements.Value.UseLayoutRounding = true;
					distributor.Distribute(elements.Value, target);

					Assert.AreEqual(target.Truncate(), elements.Value.Actual, $"Shrink Failed: target={target}");
				}
			}
		}

		private DelegateDisposableStruct<FlexElementCollection> GetElements()
		{
			return DelegateDisposableStruct.Create(_elements.MountCopy(), FlexElementCollection.Release);
		}

		[SetUp]
		public void Init()
		{
			_elements = new FlexElementCollection
			{
				new FlexElement().WithDesiredLength(10).WithActualLength(10),
				new FlexElement().WithDesiredLength(10).WithActualLength(10),
				new FlexElement().WithDesiredLength(10).WithActualLength(10),
				new FlexElement().WithDesiredLength(10).WithActualLength(10),
				new FlexElement().WithDesiredLength(10).WithActualLength(10)
			};
		}
	}
}