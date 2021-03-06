// <copyright file="XYControllerItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives
{
	public sealed class XYControllerItemCollection : UIElementCollectionBase<XYControllerItem>
	{
		internal XYControllerItemCollection(XYController xyController)
		{
			XYController = xyController;
			LogicalHost = xyController;
		}

		internal XYControllerPanel Panel
		{
			get => (XYControllerPanel) ElementsHost;
			set => ElementsHost = value;
		}

		public XYController XYController { get; }

		protected override void OnItemAdded(XYControllerItem element)
		{
			base.OnItemAdded(element);

			element.XYController = XYController;
		}

		protected override void OnItemRemoved(XYControllerItem element)
		{
			element.XYController = null;

			base.OnItemRemoved(element);
		}
	}
}