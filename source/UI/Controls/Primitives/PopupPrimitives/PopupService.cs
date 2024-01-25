// <copyright file="PopupService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal static class PopupService
	{
		public static void ClosePopupAncestors(DependencyObject dependencyObject, Popup rootPopup)
		{
			var strategy = MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance;

			foreach (var popup in dependencyObject.GetAncestors(strategy).OfType<Popup>())
			{
				popup.IsOpen = false;

				if (ReferenceEquals(popup, rootPopup))
					break;
			}
		}

		public static void ClosePopupTree(DependencyObject dependencyObject)
		{
			GetRoot(dependencyObject)?.Close();
		}

		private static IPopupSubTree GetRoot(DependencyObject dependencyObject)
		{
			return dependencyObject.GetVisualAncestorsAndSelf().OfType<IPopupSubTree>().LastOrDefault();
		}

		public static bool IsInPopupTree(DependencyObject dependencyObject)
		{
			return GetRoot(dependencyObject) != null;
		}
	}
}