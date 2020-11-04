// <copyright file="ButtonSpinner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
	public class ButtonSpinner : Spinner
	{
		#region Fields

		private readonly ButtonSpinnerTemplateContract _templateContract;

		#endregion

		#region Ctors

	  static ButtonSpinner()
	  {
	    DefaultStyleKeyHelper.OverrideStyleKey<ButtonSpinner>();
	  }

    public ButtonSpinner()
		{
			this.OverrideStyleKey<ButtonSpinner>();
			_templateContract = new ButtonSpinnerTemplateContract(GetTemplateChild, OnTemplateContractAttach, OnTemplateContractDetach);
		}

		#endregion

		#region Properties

		private Button IncreaseButton => _templateContract.IncreaseButton;

		private Button DecreaseButton => _templateContract.DecreaseButton;

		#endregion

		#region  Methods

		private void OnTemplateContractAttach()
		{
			IncreaseButton.Click += IncreaseButtonOnClick;
			DecreaseButton.Click += DecreaseButtonOnClick;
		}

		private void DecreaseButtonOnClick(object sender, RoutedEventArgs e)
		{
			OnSpin(new SpinEventArgs(SpinDirection.Decrease));
		}

		private void IncreaseButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			OnSpin(new SpinEventArgs(SpinDirection.Increase));
		}

		private void OnTemplateContractDetach()
		{
			IncreaseButton.Click -= IncreaseButtonOnClick;
			DecreaseButton.Click -= DecreaseButtonOnClick;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_templateContract.Attach();
		}

		#endregion
	}

	public class ButtonSpinnerTemplateContract : TemplateContract
	{
		#region Ctors

		public ButtonSpinnerTemplateContract(GetTemplateChild templateDiscovery, Action onAttach, Action onDetach)
			: base(templateDiscovery, onAttach, onDetach)
		{
		}

		#endregion

		#region Properties

		[PresentationCore.TemplateCore.TemplateContractPart(Required = true)]
		public Button IncreaseButton { get; [UsedImplicitly] private set; }

		[PresentationCore.TemplateCore.TemplateContractPart(Required = true)]
		public Button DecreaseButton { get; [UsedImplicitly] private set; }

		#endregion
	}
}