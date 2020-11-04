// <copyright file="ZoomValueCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ZoomValueCollection<TValue> : InheritanceContextDependencyObjectCollection<TValue> where TValue : InheritanceContextObject
	{
		public ZoomValueCollection(ZoomSelector<TValue> zoomSelector)
		{
			ZoomSelector = zoomSelector;
		}

		public ZoomSelector<TValue> ZoomSelector { get; }

		protected override void OnItemAdded(TValue obj)
		{
			base.OnItemAdded(obj);

			ZoomSelector.Update();
		}

		protected override void OnItemRemoved(TValue obj)
		{
			base.OnItemRemoved(obj);

			ZoomSelector.Update();
		}
	}
}