// <copyright file="CompositeColorProcessor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore
{
	public sealed class CompositeColorProcessor : CompositeValueProcessor<Color, CompositeColorProcessor, CompositeColorProcessorCollection>
	{
		protected override CompositeColorProcessorCollection CreateProcessorCollection()
		{
			return new CompositeColorProcessorCollection(this);
		}
	}
}