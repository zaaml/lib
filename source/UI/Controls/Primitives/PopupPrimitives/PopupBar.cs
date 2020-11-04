// <copyright file="PopupBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Pools;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public class PopupBar : PopupBarBase
	{
		private static readonly LightObjectPool<PopupBar> PopupBarPool = new LightObjectPool<PopupBar>(CreateBar, InitBar, CleanBar);

		static PopupBar()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PopupBar>();
		}

		public PopupBar()
		{
			this.OverrideStyleKey<PopupBar>();
		}

		private static void CleanBar(PopupBar popupBar)
		{
			popupBar.IsOpen = false;
			popupBar.Content = null;
		}

		private static PopupBar CreateBar()
		{
			return new PopupBar();
		}

		internal static PopupBar GetBarFromPool()
		{
			return PopupBarPool.GetObject();
		}

		private static void InitBar(PopupBar popupBar)
		{
		}

		internal static void ReleasePoolBar(PopupBar popupBar)
		{
			DelayAction.StaticInvoke(() => PopupBarPool.Release(popupBar), TimeSpan.FromSeconds(1));
		}
	}
}