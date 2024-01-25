// <copyright file="CollectionUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Runtime.CompilerServices;

namespace Zaaml.Core.Utils
{
	internal static class CollectionUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWithinRanges(int index, ICollection collection)
		{
			return index >= 0 && index < collection.Count && collection.Count > 0;
		}
	}
}