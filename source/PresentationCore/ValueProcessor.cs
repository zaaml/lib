// <copyright file="ValueProcessor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore
{
	public abstract class ValueProcessor<TValue> : InheritanceContextObject
	{
		protected virtual void OnProcessorChanged()
		{
		}

		protected abstract TValue ProcessValueCore(TValue value);

		internal TValue ProcessValueInternal(TValue value)
		{
			return ProcessValueCore(value);
		}
	}
}