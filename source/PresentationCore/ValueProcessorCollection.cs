// <copyright file="ValueProcessorCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore
{
	public class ValueProcessorCollection<TValue, TCompositeProcessor, TValueProcessorCollection> : InheritanceContextDependencyObjectCollection<ValueProcessor<TValue>>
		where TCompositeProcessor : CompositeValueProcessor<TValue, TCompositeProcessor, TValueProcessorCollection>
		where TValueProcessorCollection : ValueProcessorCollection<TValue, TCompositeProcessor, TValueProcessorCollection>
	{
		internal ValueProcessorCollection(TCompositeProcessor compositeProcessor)
		{
			CompositeProcessor = compositeProcessor;
		}

		public TCompositeProcessor CompositeProcessor { get; }
	}
}