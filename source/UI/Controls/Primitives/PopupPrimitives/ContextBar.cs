// <copyright file="ContextBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public partial class ContextBar : PopupBarBase, IContextPopupControlInternal
	{
		private static readonly DependencyPropertyKey TargetPropertyKey = DPM.RegisterAttachedReadOnly<DependencyObject, ContextBar>
			("Target", OnTargetPropertyChanged);

		public static readonly DependencyProperty TargetProperty = TargetPropertyKey.DependencyProperty;

		private bool _isShared;

		static ContextBar()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ContextBar>();
		}

		public ContextBar()
		{
			Owners = new SharedItemOwnerCollection(this);

			this.OverrideStyleKey<ContextBar>();
		}

		private bool IsShared
		{
			get => _isShared;
			set
			{
				if (_isShared == value)
					return;

				_isShared = value;

				OnIsSharedChanged();
			}
		}

		internal virtual bool OwnerAttachSelector => true;

		private SharedItemOwnerCollection Owners { get; }

		public DependencyObject Target
		{
			get => (DependencyObject)GetValue(TargetProperty);
			private set => this.SetReadOnlyValue(TargetPropertyKey, value);
		}

		public static DependencyObject GetTarget(DependencyObject dependencyObject)
		{
			return (DependencyObject)dependencyObject.GetValue(TargetProperty);
		}

		private bool IsCommandParameterContextBar(object commandParameter, out FrameworkElement frameworkElement)
		{
			frameworkElement = commandParameter as FrameworkElement;

			if (frameworkElement == null)
				return false;

			var contextBar = ContextBarService.GetContextBar(frameworkElement);

			return ReferenceEquals(this, contextBar);
		}

		protected override bool OnCanExecuteCloseCommand(object commandParameter)
		{
			return IsCommandParameterContextBar(commandParameter, out _);
		}

		protected override bool OnCanExecuteOpenCommand(object commandParameter)
		{
			return IsCommandParameterContextBar(commandParameter, out _);
		}

		protected override void OnCloseCommandExecuted(object commandParameter)
		{
			if (IsCommandParameterContextBar(commandParameter, out var fre))
				PopupController.CloseContextControl(fre, fre);
		}

		private void OnIsSharedChanged()
		{
			PlatformOnIsSharedChanged();
		}

		protected override void OnOpenCommandExecuted(object commandParameter)
		{
			if (IsCommandParameterContextBar(commandParameter, out var fre))
				PopupController.OpenContextControl(fre, fre);
		}

		private void OnTargetChanged(DependencyObject oldValue, DependencyObject newValue)
		{
			Popup?.SetReadOnlyValue(TargetPropertyKey, newValue);
		}

		private static void OnTargetPropertyChanged(DependencyObject dependencyObject, DependencyObject oldValue, DependencyObject newValue)
		{
			if (dependencyObject is ContextBar contextBar)
				contextBar.OnTargetChanged(oldValue, newValue);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Popup.SetValue(TargetPropertyKey, Target);
		}

		protected override void OnTemplateContractDetaching()
		{
			Popup.SetValue(TargetPropertyKey, null);

			base.OnTemplateContractDetaching();
		}

		partial void PlatformOnIsSharedChanged();

		FrameworkElement IContextPopupControlInternal.Owner
		{
			get => Owner;
			set => Owner = value;
		}

		DependencyObject IContextPopupControlInternal.Target
		{
			get => Target;
			set => Target = value;
		}

		bool IContextPopupControlInternal.OwnerAttachSelector => OwnerAttachSelector;

		bool ISharedItem.IsShared
		{
			get => IsShared;
			set => IsShared = value;
		}

		SharedItemOwnerCollection ISharedItem.Owners => Owners;
	}
}