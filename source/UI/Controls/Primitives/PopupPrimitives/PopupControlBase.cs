// <copyright file="PopupControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using ContentPresenter = System.Windows.Controls.ContentPresenter;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	[TemplateContractType(typeof(PopupControlBaseTemplateContract))]
	public abstract class PopupControlBase : TemplateContractContentControl, IManagedPopupControl, IPopupSubTree, IPopup, IPopupChild, IClickBarrier, ISupportInitialize
	{
		public static readonly DependencyProperty PlacementProperty = DPM.Register<PopupPlacement, PopupControlBase>
			("Placement", c => c.PopupController.OnPlacementChanged);

		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, PopupControlBase>
			("IsOpen", c => c.PopupController.OnIsOpenChanged);

		private static readonly DependencyPropertyKey OwnerPropertyKey = DPM.RegisterReadOnly<FrameworkElement, PopupControlBase>
			("Owner", c => c.PopupController.OnOwnerChanged);

		public static readonly DependencyProperty AllowOpacityAnimationProperty = DPM.Register<bool, PopupControlBase>
			("AllowOpacityAnimation", true);

		public static readonly DependencyProperty AllowMotionAnimationProperty = DPM.Register<bool, PopupControlBase>
			("AllowMotionAnimation", true);

		public static readonly DependencyProperty DropShadowProperty = DPM.Register<bool, PopupControlBase>
			("DropShadow", true);

		public static readonly DependencyProperty PlacementOptionsProperty = DPM.Register<PopupPlacementOptions, PopupControlBase>
			("PlacementOptions", PopupPlacementOptions.None);

		public static readonly DependencyProperty StaysOpenProperty = DPM.Register<bool, PopupControlBase>
			("StaysOpen", false);

		public static readonly DependencyProperty PopupMinWidthProperty = DPM.Register<PopupLength, PopupControlBase>
			("PopupMinWidth", new PopupLength(0.0));

		public static readonly DependencyProperty PopupMaxWidthProperty = DPM.Register<PopupLength, PopupControlBase>
			("PopupMaxWidth", new PopupLength(double.PositiveInfinity));

		public static readonly DependencyProperty PopupWidthProperty = DPM.Register<PopupLength, PopupControlBase>
			("PopupWidth", PopupLength.Auto);

		public static readonly DependencyProperty PopupMinHeightProperty = DPM.Register<PopupLength, PopupControlBase>
			("PopupMinHeight", new PopupLength(0.0));

		public static readonly DependencyProperty PopupMaxHeightProperty = DPM.Register<PopupLength, PopupControlBase>
			("PopupMaxHeight", new PopupLength(double.PositiveInfinity));

		public static readonly DependencyProperty PopupHeightProperty = DPM.Register<PopupLength, PopupControlBase>
			("PopupHeight", PopupLength.Auto);

		public static readonly DependencyProperty TriggerProperty = DPM.Register<PopupTrigger, PopupControlBase>
			("Trigger", d => d.OnTriggerPropertyChangedPrivate);

		public static readonly DependencyProperty ScopeProperty = DPM.Register<PopupControlScope, PopupControlBase>
			("Scope", PopupControlScope.Shared, d => d.OnScopePropertyChangedPrivate);

		public static readonly DependencyProperty OwnerProperty = OwnerPropertyKey.DependencyProperty;

		private FocusHolder _focusHolder;

		public event RoutedEventHandler Closed;
		public event RoutedEventHandler Opened;
		public event EventHandler<CancelEventArgs> Opening;
		public event EventHandler<CancelEventArgs> Closing;
		public event EventHandler OwnerChanged;

		protected PopupControlBase()
		{
			PopupController = new PopupControlController<PopupControlBase>(this);

			PopupHierarchyController.EnableHierarchy(this);

			Focusable = true;
			IsTabStop = false;

			OpenCommand = new RelayCommand(OnOpenCommandExecutedPrivate, OnCanExecuteOpenCommandPrivate);
			CloseCommand = new RelayCommand(OnCloseCommandExecutedPrivate, OnCanExecuteCloseCommandPrivate);
		}

		public bool AllowMotionAnimation
		{
			get => (bool)GetValue(AllowMotionAnimationProperty);
			set => SetValue(AllowMotionAnimationProperty, value);
		}

		public bool AllowOpacityAnimation
		{
			get => (bool)GetValue(AllowOpacityAnimationProperty);
			set => SetValue(AllowOpacityAnimationProperty, value);
		}

		public ICommand CloseCommand { get; }

		private ContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		public bool DropShadow
		{
			get => (bool)GetValue(DropShadowProperty);
			set => SetValue(DropShadowProperty, value);
		}

		internal virtual bool HandleFocus => false;

		public ICommand OpenCommand { get; }

		public FrameworkElement Owner
		{
			get => (FrameworkElement)GetValue(OwnerProperty);
			internal set => this.SetReadOnlyValue(OwnerPropertyKey, value);
		}

		public PopupPlacementOptions PlacementOptions
		{
			get => (PopupPlacementOptions)GetValue(PlacementOptionsProperty);
			set => SetValue(PlacementOptionsProperty, value);
		}

		protected Popup Popup => TemplateContract.Popup;

		internal PopupControlController<PopupControlBase> PopupController { get; }

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupHeight
		{
			get => (PopupLength)GetValue(PopupHeightProperty);
			set => SetValue(PopupHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMaxHeight
		{
			get => (PopupLength)GetValue(PopupMaxHeightProperty);
			set => SetValue(PopupMaxHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMaxWidth
		{
			get => (PopupLength)GetValue(PopupMaxWidthProperty);
			set => SetValue(PopupMaxWidthProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMinHeight
		{
			get => (PopupLength)GetValue(PopupMinHeightProperty);
			set => SetValue(PopupMinHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMinWidth
		{
			get => (PopupLength)GetValue(PopupMinWidthProperty);
			set => SetValue(PopupMinWidthProperty, value);
		}

		protected PopupContentPresenter PopupPresenter => TemplateContract.PopupPresenter;

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupWidth
		{
			get => (PopupLength)GetValue(PopupWidthProperty);
			set => SetValue(PopupWidthProperty, value);
		}

		public PopupControlScope Scope
		{
			get => (PopupControlScope)GetValue(ScopeProperty);
			set => SetValue(ScopeProperty, value);
		}

		public bool StaysOpen
		{
			get => (bool)GetValue(StaysOpenProperty);
			set => SetValue(StaysOpenProperty, value);
		}

		private PopupControlBaseTemplateContract TemplateContract => (PopupControlBaseTemplateContract)TemplateContractInternal;

		public PopupTrigger Trigger
		{
			get => (PopupTrigger)GetValue(TriggerProperty);
			set => SetValue(TriggerProperty, value);
		}

		protected sealed override Size ArrangeOverride(Size arrangeBounds)
		{
			if (IsOpen)
				base.ArrangeOverride(XamlConstants.ZeroSize);

			return XamlConstants.ZeroSize;
		}

		public void Close()
		{
			CloseCore();
		}

		protected virtual void CloseCore()
		{
			PopupController.IsOpen = false;
		}

		private void InvalidatePrivate()
		{
			InvalidateVisual();
			InvalidateMeasure();
			InvalidateArrange();
		}

		protected sealed override Size MeasureOverride(Size availableSize)
		{
			OnMeasuring();

			if (IsOpen)
				base.MeasureOverride(XamlConstants.ZeroSize);

			return XamlConstants.ZeroSize;
		}

		internal void MountContent()
		{
			ContentPresenter?.SetBinding(ContentPresenter.ContentProperty, new Binding
			{
				Path = new PropertyPath(ContentProperty),
				Source = this,
				Mode = BindingMode.OneWay
			});
		}

		protected virtual bool OnCanExecuteCloseCommand(object commandParameter)
		{
			return true;
		}

		private bool OnCanExecuteCloseCommandPrivate(object commandParameter)
		{
			return StaysOpen == false && OnCanExecuteCloseCommand(commandParameter);
		}

		protected virtual bool OnCanExecuteOpenCommand(object commandParameter)
		{
			return true;
		}

		private bool OnCanExecuteOpenCommandPrivate(object commandParameter)
		{
			return OnCanExecuteOpenCommand(commandParameter);
		}

		protected virtual void OnCloseCommandExecuted(object commandParameter)
		{
			CloseCore();
		}

		private void OnCloseCommandExecutedPrivate(object commandParameter)
		{
			if (StaysOpen == false)
				OnCloseCommandExecuted(commandParameter);
		}

		protected virtual void OnClosed()
		{
			Closed?.Invoke(this, RoutedEventArgsFactory.Create(this));
		}

		internal virtual void OnClosedInternal()
		{
		}

		private void OnClosedPrivate()
		{
			if (HandleFocus)
			{
				FocusHelper.CancelKeyboardFocusQuery(this);

				var focusedElement = FocusHelper.GetKeyboardFocusedElement() as UIElement;

				if (focusedElement?.GetAncestorsAndSelf(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance).Any(a => ReferenceEquals(a, this)) == true)
					_focusHolder.Restore();
			}

			OnClosedInternal();
			OnClosed();
		}

		protected virtual void OnClosing(PopupCancelEventArgs e)
		{
			Closing?.Invoke(this, e);
		}

		internal virtual void OnClosingInternal(PopupCancelEventArgs e)
		{
		}

		private void OnClosingPrivate(PopupCancelEventArgs e)
		{
			OnClosingInternal(e);

			if (e.Cancel)
				return;

			OnClosing(e);
		}

		//protected override void OnContentChanged(object oldContent, object newContent)
		//{
		//	base.OnContentChanged(oldContent, newContent);

		//	Popup?.Panel?.InvalidateMeasure();
		//}

		protected virtual void OnIsOpenChanged()
		{
			IsOpenChanged?.Invoke(this, EventArgs.Empty);
		}

		internal virtual void OnIsOpenChangedInternal()
		{
		}

		private void OnIsOpenChangedPrivate()
		{
			InvalidatePrivate();

			OnIsOpenChangedInternal();
			OnIsOpenChanged();
		}

		protected virtual void OnMeasuring()
		{
		}

		protected virtual void OnOpenCommandExecuted(object commandParameter)
		{
			OpenCore();
		}

		private void OnOpenCommandExecutedPrivate(object commandParameter)
		{
			OnOpenCommandExecuted(commandParameter);
		}

		protected virtual void OnOpened()
		{
			Opened?.Invoke(this, RoutedEventArgsFactory.Create(this));
		}

		internal virtual void OnOpenedInternal()
		{
		}

		private void OnOpenedPrivate()
		{
			if (HandleFocus)
			{
				_focusHolder.Save();

				FocusHelper.QueryKeyboardFocus(this);
			}

			OnOpenedInternal();
			OnOpened();
		}

		protected virtual void OnOpening(PopupCancelEventArgs e)
		{
			Opening?.Invoke(this, e);
		}

		internal virtual void OnOpeningInternal(PopupCancelEventArgs e)
		{
		}

		private void OnOpeningPrivate(PopupCancelEventArgs e)
		{
			OnOpeningInternal(e);

			if (e.Cancel)
				return;

			OnOpening(e);
		}

		protected virtual void OnOwnerChanged(FrameworkElement oldOwner, FrameworkElement newOwner)
		{
			OwnerChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnOwnerChangedPrivate(FrameworkElement oldOwner, FrameworkElement newOwner)
		{
			OnOwnerChanged(oldOwner, newOwner);
		}

		protected virtual void OnPlacementChanged(PopupPlacement oldPlacement, PopupPlacement newPlacement)
		{
			PlacementChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnPlacementChangedPrivate(PopupPlacement oldPlacement, PopupPlacement newPlacement)
		{
			OnPlacementChanged(oldPlacement, newPlacement);
		}

		private void OnScopePropertyChangedPrivate(PopupControlScope oldValue, PopupControlScope newValue)
		{
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			PopupController.Popup = Popup;

			if (Trigger != null)
				Trigger.Popup = Popup;

			MountContent();

			// TODO DropDownBar refactoring. Investigate
			//Popup.TreeMode = PopupTreeMode.Detached;
			//PopupController.OnPopupTreeModeChanged();
		}

		protected override void OnTemplateContractDetaching()
		{
			if (Trigger != null)
				Trigger.Popup = null;

			ReleaseContent();

			PopupController.Popup = null;

			base.OnTemplateContractDetaching();
		}

		private void OnTriggerActualIsOpenChanged(object sender, EventArgs e)
		{
			IsOpen = Trigger.ActualIsOpen;
		}

		private void OnTriggerPropertyChangedPrivate(PopupTrigger oldValue, PopupTrigger newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				oldValue.ActualIsOpenChanged -= OnTriggerActualIsOpenChanged;
				oldValue.Popup = null;
			}

			if (newValue != null)
			{
				newValue.ActualIsOpenChanged += OnTriggerActualIsOpenChanged;
				newValue.Popup = Popup;
			}
		}

		public void Open()
		{
			OpenCore();
		}

		protected virtual void OpenCore()
		{
			PopupController.IsOpen = true;
		}

		internal void ReleaseContent()
		{
			ContentPresenter?.ClearValue(ContentPresenter.ContentProperty);
		}

		bool IClickBarrier.PropagateEvents => false;

		void IManagedPopupControl.OnOwnerChanged(FrameworkElement oldOwner, FrameworkElement newOwner)
		{
			OnOwnerChangedPrivate(oldOwner, newOwner);
		}

		void IManagedPopupControl.OnPlacementChanged(PopupPlacement oldPlacement, PopupPlacement newPlacement)
		{
			OnPlacementChangedPrivate(oldPlacement, newPlacement);
		}

		DependencyProperty IManagedPopupControl.IsOpenProperty => IsOpenProperty;

		DependencyPropertyKey IManagedPopupControl.OwnerPropertyKey => OwnerPropertyKey;

		DependencyProperty IManagedPopupControl.PlacementProperty => PlacementProperty;

		void IManagedPopupControl.OnClosing(PopupCancelEventArgs e)
		{
			OnClosingPrivate(e);
		}

		void IManagedPopupControl.OnOpening(PopupCancelEventArgs e)
		{
			OnOpeningPrivate(e);
		}

		void IManagedPopupControl.OnOpened()
		{
			OnOpenedPrivate();
		}

		void IManagedPopupControl.OnClosed()
		{
			OnClosedPrivate();
		}

		void IManagedPopupControl.OnIsOpenChanged()
		{
			OnIsOpenChangedPrivate();
		}

		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		public PopupPlacement Placement
		{
			get => (PopupPlacement)GetValue(PlacementProperty);
			set => SetValue(PlacementProperty, value);
		}

		public event EventHandler IsOpenChanged;
		public event EventHandler PlacementChanged;

		void IPopupSubTree.Close()
		{
			Close();
		}

		void ISupportInitialize.BeginInit()
		{
		}

		void ISupportInitialize.EndInit()
		{
		}
	}

	public class PopupControlBaseTemplateContract : TemplateContract
	{
		[TemplateContractPart] public ContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public Popup Popup { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public PopupContentPresenter PopupPresenter { get; [UsedImplicitly] private set; }
	}
}