// <copyright file="BackstageApplicationMenu.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Menu;

namespace Zaaml.UI.Controls.BackstageView
{
	[ContentProperty(nameof(BackstageViewControl))]
	public class BackstageApplicationMenu : ApplicationMenu
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty BackstageViewControlProperty = DPM.Register<BackstageViewControl, BackstageApplicationMenu>
			(nameof(BackstageViewControl), m => m.OnBackstageChanged);

		#endregion

		#region Properties

		public BackstageViewControl BackstageViewControl
		{
			get => (BackstageViewControl) GetValue(BackstageViewControlProperty);
			set => SetValue(BackstageViewControlProperty, value);
		}

		protected override IEnumerator LogicalChildren => BackstageViewControl != null ? EnumeratorUtils.Concat(BackstageViewControl, base.LogicalChildren) : base.LogicalChildren;

		#endregion

		#region  Methods

		private void OnBackstageChanged(BackstageViewControl oldBackstage, BackstageViewControl newBackstage)
		{
			if (oldBackstage != null)
			{
				oldBackstage.ClearValue(BackstageViewControl.IsOpenProperty);

				RemoveLogicalChild(oldBackstage);
			}

			if (newBackstage != null)
			{
				AddLogicalChild(newBackstage);

				newBackstage.SetBinding(BackstageViewControl.IsOpenProperty, new Binding { Path = new PropertyPath(IsOpenProperty), Source = this, Mode = BindingMode.TwoWay });
			}
		}

		#endregion
	}
}