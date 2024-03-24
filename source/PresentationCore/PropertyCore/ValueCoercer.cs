// <copyright file="ValueCoercer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.PropertyCore
{
	public abstract class ValueCoercer<T> : InheritanceContextObject
	{
		public abstract T CoerceValue(T value);
	}
}