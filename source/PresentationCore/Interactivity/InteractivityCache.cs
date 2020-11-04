// <copyright file="InteractivityCache.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.Interactivity
{
	//internal class InteractivityCache
	//{
	//	#region Static Fields and Constants

	//	private static readonly object UnsetValue = new object();

	//	#endregion

	//	#region Ctors

	//	public InteractivityCache(object originalValue, Type targetType)
	//	{
	//		OriginalValue = originalValue;
	//		UpdateCache(targetType);
	//	}

	//	#endregion

	//	#region Properties

	//	public object CachedValue { get; private set; }

	//	public object OriginalValue { get; }

	//	#endregion

	//	#region  Methods

	//	public object GetValue(Type targetType)
	//	{
	//		UpdateCache(targetType);
	//		return CachedValue;
	//	}

	//	private void UpdateCache(Type targetType)
	//	{
	//		if (ReferenceEquals(CachedValue, UnsetValue) == false && CachedValue?.GetType() == targetType) return;
	//		CachedValue = XamlStaticConverter.ConvertValue(OriginalValue, targetType);
	//	}

	//	#endregion
	//}
}