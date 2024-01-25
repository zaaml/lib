// <copyright file="NavigationViewHeaderedIconItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewHeaderedIconItemTemplateContract))]
	public abstract class NavigationViewHeaderedIconItem : NavigationViewItemBase
	{
		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, NavigationViewHeaderedIconItem>
			("Header");

		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, NavigationViewHeaderedIconItem>
			("Icon");

		private protected virtual ClickMode ClickModeCore => ClickMode.Release;

		internal ClickMode ClickModeInternal => ClickModeCore;

		private protected virtual ICommand CommandCore => null;

		internal ICommand CommandInternal => CommandCore;

		private protected virtual object CommandParameterCore => null;

		internal object CommandParameterInternal => CommandParameterCore;

		private protected virtual DependencyObject CommandTargetCore => null;

		internal DependencyObject CommandTargetInternal => CommandTargetCore;

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		protected NavigationViewHeaderedIconItemPresenter HeaderedIconItemPresenter => TemplateContract.HeaderedIconItemPresenter;

		public IconBase Icon
		{
			get => (IconBase) GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		private protected virtual bool IsPressedCore
		{
			get => false;
			set { }
		}

		internal bool IsPressedInternal
		{
			get => IsPressedCore;
			set => IsPressedCore = value;
		}

		private NavigationViewHeaderedIconItemTemplateContract TemplateContract => (NavigationViewHeaderedIconItemTemplateContract)TemplateContractCore;

		private protected virtual void OnClickCore()
		{
		}

		internal void OnClickInternal()
		{
			OnClickCore();

			NavigationViewControl?.OnItemClick(this);
		}

		private protected override void OnCommandChangedCore()
		{
			base.OnCommandChangedCore();

			HeaderedIconItemPresenter?.OnCommandChanged();
		}

		private protected override void OnCommandParameterChangedCore()
		{
			base.OnCommandParameterChangedCore();

			HeaderedIconItemPresenter?.OnCommandParameterChanged();
		}

		private protected override void OnCommandTargetChangedCore()
		{
			base.OnCommandTargetChangedCore();

			HeaderedIconItemPresenter?.OnCommandTargetChanged();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			HeaderedIconItemPresenter.NavigationViewItem = this;
		}

		protected override void OnTemplateContractDetaching()
		{
			HeaderedIconItemPresenter.NavigationViewItem = null;

			base.OnTemplateContractDetaching();
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			base.UpdateVisualState(useTransitions);

			HeaderedIconItemPresenter?.UpdateVisualStateInternal(useTransitions);
		}
	}
}