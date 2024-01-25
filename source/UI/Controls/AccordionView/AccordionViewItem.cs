// <copyright file="AccordionViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.AccordionView
{
	public class AccordionViewItem : HeaderedIconContentControl, ISelectionStateControl
	{
		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, AccordionViewItem>
			("IsSelected");

		private Button _headerButton;

		static AccordionViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<AccordionViewItem>();
		}

		public AccordionViewItem()
		{
			this.OverrideStyleKey<AccordionViewItem>();
		}

		internal AccordionViewControl AccordionViewControl { get; set; }

		private Button HeaderButton
		{
			get => _headerButton;
			set
			{
				if (ReferenceEquals(_headerButton, value))
					return;

				if (_headerButton != null)
					_headerButton.Click -= HeaderButtonOnClick;

				_headerButton = value;

				if (_headerButton != null)
					_headerButton.Click += HeaderButtonOnClick;
			}
		}

		private void HeaderButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			IsSelected = !IsSelected;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			HeaderButton = (Button) GetTemplateChild("HeaderButton");
		}

		protected override void OnTemplateContractDetaching()
		{
			HeaderButton = null;

			base.OnTemplateContractDetaching();
		}

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value.Box());
		}
	}
}