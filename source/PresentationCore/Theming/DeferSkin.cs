// <copyright file="DeferSkin.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Zaaml.PresentationCore.Theming
{
	internal sealed class DeferSkin : SkinBase
	{
		public string Key { get; set; }

		internal override IEnumerable<KeyValuePair<string, object>> Resources => Enumerable.Empty<KeyValuePair<string, object>>();

		protected override object GetValue(string key)
		{
			return null;
		}
	}
}