// <copyright file="DocumentLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class DocumentLayout : TabLayoutBase
	{
		public static readonly DependencyProperty OrderProperty = DPM.RegisterAttached<int, DocumentLayout>
			("Order", 0, OnOrderPropertyChanged);

		private static readonly List<DependencyProperty> DocumentLayoutProperties = new()
		{
			OrderProperty
		};

		static DocumentLayout()
		{
			RegisterLayoutProperties<DocumentLayout>(DocumentLayoutProperties);
		}

		public override LayoutKind LayoutKind => LayoutKind.Document;

		public static int GetOrder(DependencyObject depObj)
		{
			return (int)depObj.GetValue(OrderProperty);
		}

		protected override int GetDockItemOrder(DockItem dockItem)
		{
			return GetOrder(dockItem);
		}

		private void OnItemOrderChanged(DockItem dockItem)
		{
		}

		private static void OnOrderChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			if (depObj is not DockItem dockItem)
				return;

			var documentLayout = dockItem.ActualLayout as DocumentLayout;

			documentLayout?.OnItemOrderChanged(dockItem);
		}

		private static void OnOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OnOrderChanged(d, e);
			OnLayoutPropertyChanged(d, e);
		}

		public static void SetOrder(DependencyObject depObj, int orderIndex)
		{
			depObj.SetValue(OrderProperty, orderIndex);
		}
	}
}