using System.Linq;
using System.Threading;
using NUnit.Framework;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.SplitView;
using Zaaml.UI.Test;

namespace Zaaml.UI.Navigation.Test.NavigationView
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public class NavigationViewTest : UITestBase<App>
	{
		[Test]
		public void Test()
		{
			var mainWindow = new NavigationViewTestWindow();

			mainWindow.Show();
			mainWindow.UpdateLayout();

			var splitViewControl = mainWindow.NavigationViewControl.GetVisualDescendants<SplitViewControl>().Single();

			Assert.Less(splitViewControl.ActualPaneLength, 100);
		}
	}
}