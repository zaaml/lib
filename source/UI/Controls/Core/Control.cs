// <copyright file="Control.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Extensions;
using NativeControl = System.Windows.Controls.Control;
using NativeVisualStateManager = System.Windows.VisualStateManager;

namespace Zaaml.UI.Controls.Core
{
	public partial class Control : NativeControl, IControl, ILogicalOwner, ILogicalMentorOwner, IDependencyPropertyChangedInvocator
	{
		private LogicalChildMentor<Control> _logicalChildMentor;

		internal event DependencyPropertyChangedEventHandler DependencyPropertyChangedInternal;

		public Control()
		{
			PlatformCtor();

			IsEnabledChanged += (sender, args) => OnIsEnabledChanged();
		}

		private protected LogicalChildMentor LogicalChildMentor => _logicalChildMentor ??= LogicalChildMentor.Create(this);

		protected override IEnumerator LogicalChildren => _logicalChildMentor == null ? base.LogicalChildren : _logicalChildMentor.GetLogicalChildren();

		internal DependencyObject GetTemplateChildInternal(string name)
		{
			return GetTemplateChild(name);
		}

		protected virtual bool GotoVisualState(string stateName, bool useTransitions)
		{
			return NativeVisualStateManager.GoToState(this, stateName, useTransitions);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return this.OnMeasureOverride(base.MeasureOverride, availableSize);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			UpdateVisualState(false);
		}

		private protected virtual void OnDependencyPropertyChangedInternal(DependencyPropertyChangedEventArgs args)
		{
		}

		private void OnDependencyPropertyChangedPrivate(DependencyPropertyChangedEventArgs args)
		{
			OnDependencyPropertyChangedInternal(args);

			DependencyPropertyChangedInternal?.Invoke(this, args);
		}

		protected virtual void OnIsEnabledChanged()
		{
			UpdateVisualState(true);
		}

		protected virtual void OnLoaded()
		{
		}

		protected virtual void OnUnloaded()
		{
		}

		partial void PlatformCtor();

		protected virtual void UpdateVisualState(bool useTransitions)
		{
			this.UpdateVisualGroups(useTransitions);
		}

		void IDependencyPropertyChangedInvocator.InvokeDependencyPropertyChangedEvent(DependencyPropertyChangedEventArgs args)
		{
			OnDependencyPropertyChangedPrivate(args);
		}

		void ILogicalMentorOwner.RemoveLogicalChild(object child)
		{
			RemoveLogicalChild(child);
		}

		IEnumerator ILogicalMentorOwner.BaseLogicalChildren => base.LogicalChildren;

		void ILogicalMentorOwner.AddLogicalChild(object child)
		{
			AddLogicalChild(child);
		}

		void ILogicalOwner.AddLogicalChild(object child)
		{
			LogicalChildMentor.AddLogicalChild(child);
		}

		void ILogicalOwner.RemoveLogicalChild(object child)
		{
			LogicalChildMentor.RemoveLogicalChild(child);
		}

		IEnumerator ILogicalOwner.BaseLogicalChildren => base.LogicalChildren;
	}

	internal interface IZaamlControl
	{
		IInteractivityService InteractivityService { get; }
	}
}