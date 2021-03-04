// <copyright file="CompositeValueProcessor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	public abstract class CompositeValueProcessor<TValue, TCompositeProcessor, TValueProcessorCollection> : ValueProcessor<TValue>
		where TCompositeProcessor : CompositeValueProcessor<TValue, TCompositeProcessor, TValueProcessorCollection>
		where TValueProcessorCollection : ValueProcessorCollection<TValue, TCompositeProcessor, TValueProcessorCollection>
	{
		private static readonly DependencyPropertyKey ProcessorsPropertyKey = DPM.RegisterReadOnly<TValueProcessorCollection, CompositeValueProcessor<TValue, TCompositeProcessor, TValueProcessorCollection>>
			("ProcessorsPrivate");

		public static readonly DependencyProperty ProcessorsProperty = ProcessorsPropertyKey.DependencyProperty;

		public TValueProcessorCollection Processors => this.GetValueOrCreate(ProcessorsPropertyKey, CreateProcessorCollection);

		protected abstract TValueProcessorCollection CreateProcessorCollection();

		protected override TValue ProcessValueCore(TValue value)
		{
			foreach (var processor in Processors)
				value = processor.ProcessValueInternal(value);

			return value;
		}
	}
}