// <copyright file="RibbonWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Windows;

namespace Zaaml.UI.Controls.Ribbon
{
	public class RibbonWindow : WindowBase
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty RibbonProperty = DPM.Register<RibbonControl, RibbonWindow>
			("Ribbon", w => w.OnRibbonChanged);

		#endregion

		#region Ctors

		static RibbonWindow()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<RibbonWindow>();
		}

		public RibbonWindow()
		{
			this.OverrideStyleKey<RibbonWindow>();
		}

		#endregion

		#region Properties

		protected override IEnumerator LogicalChildren => Ribbon != null ? EnumeratorUtils.Concat(Ribbon, base.LogicalChildren) : base.LogicalChildren;

		public RibbonControl Ribbon
		{
			get => (RibbonControl) GetValue(RibbonProperty);
			set => SetValue(RibbonProperty, value);
		}

		#endregion

		#region  Methods

		private void OnRibbonChanged(RibbonControl oldRibbon, RibbonControl newRibbon)
		{
			if (oldRibbon != null)
				RemoveLogicalChild(oldRibbon);

			if (newRibbon != null)
				AddLogicalChild(newRibbon);
		}

		#endregion
	}
}