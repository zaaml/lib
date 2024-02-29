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