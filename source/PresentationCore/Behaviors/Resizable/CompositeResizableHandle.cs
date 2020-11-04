// <copyright file="CompositeResizableHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.ObservableCollections;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	[ContentProperty("HandleCollection")]
	internal class CompositeResizableHandle : ResizableHandleBase
	{
		private static readonly DependencyPropertyKey HandleCollectionPropertyKey = DPM.RegisterAttachedReadOnly<CompositeResizableHandle, ResizableHandleCollection>
			("HandleCollectionPrivate");

		internal static readonly DependencyProperty HandleCollectionProperty = HandleCollectionPropertyKey.DependencyProperty;

		[UsedImplicitly] private ObservableCollectionDispatcher<ResizableHandleBase> _dispatcher;

		private ResizableHandleBase CurrentHandle { get; set; }

		public override Point CurrentLocation => new Point();

		public ResizableHandleCollection HandleCollection
		{
			get { return this.GetValueOrCreate(HandleCollectionPropertyKey, () => AttachEvents(new ResizableHandleCollection())); }
		}

		protected override ResizableHandleKind HandleKindCore { get; set; }

		public override Point OriginLocation => new Point();

		protected override void Attach(ResizableBehavior behavior)
		{
			base.Attach(behavior);

			foreach (var handle in HandleCollection)
				handle.Behavior = behavior;
		}

		private ResizableHandleCollection AttachEvents(ResizableHandleCollection collection)
		{
			_dispatcher = collection.Dispatch(OnAdded, OnRemoved);

			return collection;
		}

		protected override void Detach(ResizableBehavior behavior)
		{
			foreach (var handle in HandleCollection)
				handle.Behavior = null;

			base.Detach(behavior);
		}

		private void OnAdded(ResizableHandleBase handle)
		{
			handle.Behavior = Behavior;
		}

		private void OnRemoved(ResizableHandleBase handle)
		{
			handle.Behavior = null;
		}
	}
}