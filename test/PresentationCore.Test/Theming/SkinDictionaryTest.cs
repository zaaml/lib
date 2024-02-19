// <copyright file="SkinDictionaryTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Test.Theming
{
	internal sealed class SkinDictionaryTest
	{
		#region  Methods

		[Test(Description = "BaseOperations")]
		public void BaseOperations()
		{
			const string key = "MetroUI.Controls.Value";

			var skinDictionary = new SkinDictionary
			{
				[key] = 5
			};

			// Contains test
			Assert.AreEqual(true, skinDictionary.ContainsKey(key));

			// Add value test
			Assert.AreEqual(5, skinDictionary[key]);

			var keysHashSet = new HashSet<string>(new[] {"MetroUI", "MetroUI.Controls", "MetroUI.Controls.Value"});

			// Flatten test
			Assert.AreEqual(keysHashSet, new HashSet<string>(skinDictionary.Flatten().Select(kv => kv.Key)));

			// Remove test
			Assert.AreEqual(true, skinDictionary.Remove(key));

			// Contains test
			Assert.AreEqual(false, skinDictionary.ContainsKey(key));
		}

		[Test(Description = "MergeDefault")]
		public void MergeDefault()
		{
			var target = new SkinDictionary();

			var first = new SkinDictionary
			{
				["Button.Value1"] = 10,
				["Button.Value2"] = 15
			};

			var second = new SkinDictionary
			{
				["Button.Value3"] = 20,
			};

			target.Merge("MetroUI.Controls", first, SkinDictionaryMergeFlags.Default);
			target.Merge("MetroUI.Controls", second, SkinDictionaryMergeFlags.Default);

			Assert.AreEqual(10, target["MetroUI.Controls.Button.Value1"]);
			Assert.AreEqual(15, target["MetroUI.Controls.Button.Value2"]);
			Assert.AreEqual(20, target["MetroUI.Controls.Button.Value3"]);
		}

		[Test(Description = "MergeOverride")]
		public void MergeOverride()
		{
			var first = new SkinDictionary
			{
				["Button.Value1"] = 5,
				["Button.Value2"] = 10
			};

			var second = new SkinDictionary
			{
				["Button.Value2"] = 15,
			};

			var target = new SkinDictionary();

			target.Merge("MetroUI.Controls", first, SkinDictionaryMergeFlags.Override);

			Assert.AreEqual(5, target["MetroUI.Controls.Button.Value1"]);
			Assert.AreEqual(10, target["MetroUI.Controls.Button.Value2"]);

			target.Merge("MetroUI.Controls", second, SkinDictionaryMergeFlags.Override);

			Assert.AreEqual(5, target["MetroUI.Controls.Button.Value1"]);
			Assert.AreEqual(15, target["MetroUI.Controls.Button.Value2"]);
		}

		#endregion
	}
}