// <copyright file="SkinResourceManagerTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Reflection;
using System.Windows;
using System.Windows.Media;
using NUnit.Framework;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Test.Theming.SkinResourceManager
{
	[TestFixture]
	public class SkinResourceManagerTest
	{
		private static TestTheme Theme => TestTheme.Instance;

		[Test(Description = "Basic Test")]
		public void BasicTest()
		{
			var theme = Theme;
			var skinResourceManager = new PresentationCore.Theming.SkinResourceManager(theme);
			var basicTestUri = Assembly.GetExecutingAssembly().GetResourceUri("/Theming/SkinResourceManager/BasicTest.xaml");

			var themeSkinResourceDictionary = (ThemeSkinResourceDictionary)Application.LoadComponent(basicTestUri);

			skinResourceManager.LoadResources(themeSkinResourceDictionary);

			var baseValue1 = skinResourceManager.GetResource("BaseValues.Value1");
			var baseValue2 = skinResourceManager.GetResource("BaseValues.Value2");

			Assert.AreEqual(1, baseValue1.Value, "Value1");
			Assert.AreEqual(2, baseValue2.Value, "Value2");

			var baseNestedValue1 = skinResourceManager.GetResource("BaseValues.Nested.Value1");
			var baseNestedValue2 = skinResourceManager.GetResource("BaseValues.Nested.Value2");

			Assert.AreEqual(11, baseNestedValue1.Value, "Value1");
			Assert.AreEqual(12, baseNestedValue2.Value, "Value2");

			var finalValue1 = skinResourceManager.GetResource("FinalValues.Value1");
			var finalValue2 = skinResourceManager.GetResource("FinalValues.Value2");

			Assert.AreEqual(1, finalValue1.Value, "Value1");
			Assert.AreEqual(3, finalValue2.Value, "Value2");

			var finalNestedValue1 = skinResourceManager.GetResource("FinalValues.Nested.Value1");
			var finalNestedValue2 = skinResourceManager.GetResource("FinalValues.Nested.Value2");

			Assert.AreEqual(14, finalNestedValue1.Value, "Value1");
			Assert.AreEqual(12, finalNestedValue2.Value, "Value2");
		}

		[Test(Description = "Test Dependency Resolver")]
		public void TestDependencyResolver()
		{
			//var theme = new Theme();
			//var skinResourceManager = new SkinResourceManager(theme);

			//skinResourceManager.LoadResources(new List<string>
			//{
			//	"BaseValues",
			//	"FinalDependency",
			//	"B_Dependency",
			//	"A_Dependency",
			//	"C_Dependency",
			//}.Select(k => _interactivityResourceDictionary.CreateThemeResource(k)));

			//var intValue = skinResourceManager.GetResource("BaseValues.IntValue");

			//Assert.AreEqual(32, intValue.Value, "Base Value");

			//var intFinal = skinResourceManager.GetResource("FinalDependency.IntValue");

			//Assert.AreEqual(32, intFinal.Value, "Final Value");
		}

		[Test(Description = "ToolBar Test")]
		public void ToolBarTest()
		{
			var theme = Theme;
			var skinResourceManager = new PresentationCore.Theming.SkinResourceManager(theme);
			var toolBarTestUri = Assembly.GetExecutingAssembly().GetResourceUri("/Theming/SkinResourceManager/ToolBarTest.xaml");
			var themeSkinResourceDictionary = (ThemeSkinResourceDictionary)Application.LoadComponent(toolBarTestUri);

			skinResourceManager.LoadResources(themeSkinResourceDictionary);

			var inputValue = (Color)skinResourceManager.GetResource("MetroUI.AppToolBarControl.ToolBarSplitButton.Input.ForegroundBase").Value;
			var dropDownInputValue = (Color)skinResourceManager.GetResource("MetroUI.AppToolBarControl.ToolBarSplitButton.DropDownButton.Input.ForegroundBase").Value;
			var foregroundNormalValue = (SolidColorBrush)skinResourceManager.GetResource("MetroUI.AppToolBarControl.ToolBarSplitButton.DropDownButton.Foreground.Normal").Value;

			Assert.AreEqual(Color.FromArgb(255, 255, 255, 255), inputValue);
			Assert.AreEqual(Color.FromArgb(255, 255, 255, 255), dropDownInputValue);
			Assert.AreEqual(Color.FromArgb(255, 255, 255, 255), foregroundNormalValue.Color);
		}

		private class TestTheme : SkinnedTheme
		{
			private TestTheme()
			{
			}

			public static readonly TestTheme Instance = new();

			protected override Theme BaseThemeCore => null;

			public override string Name => "Theme";

			protected override string SkinName => "Skin";
		}
	}
}