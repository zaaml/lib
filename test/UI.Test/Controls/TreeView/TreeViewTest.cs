// <copyright file="TreeViewTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NUnit.Framework;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.TreeView;
using TreeViewItem = Zaaml.UI.Controls.TreeView.TreeViewItem;

namespace Zaaml.UI.Test.Controls.TreeView
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public class TreeViewTest : UITestBase<App>
	{
		[Test]
		public void TestTreeViewItemIcon()
		{
			var treeViewControl = new TreeViewControl
			{
				SourceCollection = new[] { new TreeItemData() },
				ItemIconMember = nameof(TreeItemData.Icon)
			};

			RenderElement(treeViewControl);

			var treeViewItem = treeViewControl.GetVisualDescendants<TreeViewItem>().Single();
			var icon = treeViewItem.Icon;

			Assert.IsInstanceOf<BitmapIcon>(icon);

			var bitmapIcon = (BitmapIcon)icon;

			Assert.IsInstanceOf<WriteableBitmap>(bitmapIcon.Source);
		}

		private sealed class TreeItemData
		{
			public ImageSource Icon { get; set; } = new WriteableBitmap(15, 15, 96, 96, PixelFormats.Rgb24, null);
		}
	}
}