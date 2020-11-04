// <copyright file="ThemeResourceDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
{
	public sealed class ThemeResourceDictionary : ResourceDictionaryBase
	{
		#region Properties

		internal bool IsDeferred { get; set; }

		#endregion

		#region  Methods

		internal ThemeResourceDictionary Clone()
		{
			var themeResourceDictionary = new ThemeResourceDictionary();

			foreach (var key in Keys)
				themeResourceDictionary.Add(key, this[key]);

			return themeResourceDictionary;
		}

		#endregion
	}
}