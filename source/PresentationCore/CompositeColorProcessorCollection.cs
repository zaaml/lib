// <copyright file="CompositeColorProcessorCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore
{
	public sealed class CompositeColorProcessorCollection : ValueProcessorCollection<Color, CompositeColorProcessor, CompositeColorProcessorCollection>
	{
		internal CompositeColorProcessorCollection(CompositeColorProcessor compositeProcessor) : base(compositeProcessor)
		{
		}
	}
}