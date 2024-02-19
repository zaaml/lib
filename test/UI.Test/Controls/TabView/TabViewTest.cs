// <copyright file="TabViewTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NUnit.Framework;
using Zaaml.PresentationCore.Automation;
using Zaaml.PresentationCore.Input;

namespace Zaaml.UI.Test.Controls.TabView
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public class TabViewTest : UITestBase<App>
	{
		[Test]
		public void Test()
		{
			//var mainWindow = new TabViewTestWindow();

			//async void OnMainWindowOnContentRendered(object o, EventArgs eventArgs)
			//{
			//	mainWindow.UpdateLayout();

			//	await Task.Delay(1000);

			//	var mouseAutomation = new MouseAutomation(new MouseAutomationOptions
			//	{
			//		MoveDuration = TimeSpan.FromMilliseconds(100),
			//		PreEventDelay = TimeSpan.FromMilliseconds(100),
			//		PostEventDelay = TimeSpan.FromMilliseconds(100),
			//	});

			//	var keyboardAutomation = new KeyboardAutomation(new KeyboardAutomationOptions
			//	{
			//		PreEventDelay = TimeSpan.FromMilliseconds(10),
			//		PostEventDelay = TimeSpan.FromMilliseconds(10)
			//	});

			//	mainWindow.TextBox1.Focus();

			//	await keyboardAutomation.PressAsync(Key.A);
			//	await keyboardAutomation.PressAsync(Key.B);
			//	await keyboardAutomation.PressAsync(Key.C);

			//	mainWindow.Close();
			//}

			//mainWindow.ContentRendered += OnMainWindowOnContentRendered;

			//mainWindow.Show();

			//Application.Current.Run(mainWindow);
		}
	}
}