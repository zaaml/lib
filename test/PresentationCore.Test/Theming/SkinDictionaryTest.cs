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
			Assert.That(skinDictionary.ContainsKey(key), Is.EqualTo(true));

			// Add value test
			Assert.That(skinDictionary[key], Is.EqualTo(5));

			var keysHashSet = new HashSet<string>(new[] {"MetroUI", "MetroUI.Controls", "MetroUI.Controls.Value"});

			// Flatten test
			Assert.That(new HashSet<string>(skinDictionary.Flatten().Select(kv => kv.Key)), Is.EqualTo(keysHashSet));

			// Remove test
			Assert.That(skinDictionary.Remove(key), Is.EqualTo(true));

			// Contains test
			Assert.That(skinDictionary.ContainsKey(key), Is.EqualTo(false));
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

			Assert.That(target["MetroUI.Controls.Button.Value1"], Is.EqualTo(10));
			Assert.That(target["MetroUI.Controls.Button.Value2"], Is.EqualTo(15));
			Assert.That(target["MetroUI.Controls.Button.Value3"], Is.EqualTo(20));
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

			Assert.That(target["MetroUI.Controls.Button.Value1"], Is.EqualTo(5));
			Assert.That(target["MetroUI.Controls.Button.Value2"], Is.EqualTo(10));

			target.Merge("MetroUI.Controls", second, SkinDictionaryMergeFlags.Override);

			Assert.That(target["MetroUI.Controls.Button.Value1"], Is.EqualTo(5));
			Assert.That(target["MetroUI.Controls.Button.Value2"], Is.EqualTo(15));
		}

		#endregion
	}
}