// <copyright file="TrackBarControlTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading;
using NUnit.Framework;
using Zaaml.UI.Controls.Primitives.TrackBar;

namespace Zaaml.UI.Test.Controls.Primitives.TrackBar
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public class TrackBarControlTest : UITestBase<App>
	{
		[Test]
		public void UnorderedAppend()
		{
			var trackBar = new TrackBarControl
			{
				Minimum = 0,
				Maximum = 100
			};

			trackBar.ItemCollection.Add(new TrackBarRangeItem());
			trackBar.ItemCollection.Add(new TrackBarValueItem { Value = 50 });
			trackBar.ItemCollection.Add(new TrackBarRangeItem());
			trackBar.ItemCollection.Add(new TrackBarValueItem { Value = 60 });
			trackBar.ItemCollection.Add(new TrackBarRangeItem());
			trackBar.ItemCollection.Add(new TrackBarValueItem { Value = 10 });
			trackBar.ItemCollection.Add(new TrackBarRangeItem());

			var values = trackBar.ItemCollection.OfType<TrackBarValueItem>().Select(x => x.Value).ToArray();
			var ranges = trackBar.ItemCollection.OfType<TrackBarRangeItem>().Select(x => x.Range).ToArray();

			Assert.True(values.SequenceEqual([50, 60, 60]));
			Assert.True(ranges.SequenceEqual([50, 10, 0, 40]));
		}

		[Test]
		public void SetMinimum()
		{
			var trackBar = new TrackBarControl
			{
				ItemCollection =
				{
					new TrackBarValueItem()
				}
			};

			trackBar.Minimum = trackBar.Maximum + 1;
		}

		[Test]
		public void SetMaximum()
		{
			var trackBar = new TrackBarControl
			{
				ItemCollection =
				{
					new TrackBarValueItem()
				}
			};

			trackBar.Maximum = trackBar.Minimum - 1;
		}

		[Test]
		public void DualValueItem()
		{
			var startRange = new TrackBarRangeItem();
			var middleRange = new TrackBarRangeItem();
			var endRange = new TrackBarRangeItem();

			var startValue = new TrackBarValueItem();
			var endValue = new TrackBarValueItem();

			var trackBar = new TrackBarControl
			{
				ItemCollection =
				{
					startRange,
					startValue,
					middleRange,
					endValue,
					endRange
				}
			};

			trackBar.BeginUpdate();

			trackBar.Minimum = 900;
			trackBar.Maximum = 1000;
			endValue.Value = 950;

			trackBar.EndUpdate();

			Assert.AreEqual(900, startValue.Value);
			Assert.AreEqual(950, endValue.Value);
		}

		[Test]
		public void RangesOnly()
		{
			var trackBar = new TrackBarControl
			{
				Minimum = 0,
				Maximum = 100
			};

			trackBar.ItemCollection.Add(new TrackBarRangeItem());

			var ranges = trackBar.ItemCollection.OfType<TrackBarRangeItem>().Select(x => x.Range).ToArray();

			Assert.True(ranges.SequenceEqual([100]));

			trackBar.ItemCollection.Add(new TrackBarRangeItem());
			trackBar.ItemCollection.Add(new TrackBarRangeItem());
			trackBar.ItemCollection.Add(new TrackBarRangeItem());

			ranges = trackBar.ItemCollection.OfType<TrackBarRangeItem>().Select(x => x.Range).ToArray();

			Assert.True(ranges.SequenceEqual([25, 25, 25, 25]));
		}
	}
}