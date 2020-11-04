// <copyright file="VisualStateListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.Core.Weak;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.PresentationCore.Interactivity
{
	internal class VisualStateListenerBase : IDisposable
	{
		#region Fields

		private readonly Control _control;
		private readonly string _visualStateName;
		private string _currentStateName;
		private bool _isStateDirty;
		private FrameworkElement _templateRoot;
		private IDisposable _templateRootHandlersDisposer;
		private IDisposable _visualGroupHandlersDisposer;
		private VisualStateGroup _visualStateGroup;

		#endregion

		#region Ctors

		public VisualStateListenerBase(Control control, string visualStateName)
		{
			_control = control;
			_visualStateName = visualStateName;
			_control.LayoutUpdated += ControlOnLayoutUpdated;

			UpdateTemplateRoot();
		}

		#endregion

		#region Properties

		public string CurrentStateName
		{
			get => _currentStateName;
			private set
			{
				if (string.Equals(_currentStateName, value))
					return;

				_currentStateName = value;

				if (_currentStateName == _visualStateName)
					OnVisualStateEntered();
				else
					OnVisualStateLeaved();
			}
		}

		private FrameworkElement TemplateRoot
		{
			get => _templateRoot;
			set
			{
				if (ReferenceEquals(_templateRoot, value))
					return;

				_templateRootHandlersDisposer = _templateRootHandlersDisposer.DisposeExchange();

				_templateRoot = value;

				if (_templateRoot != null)
				{
					var tr = _templateRoot;
					_templateRootHandlersDisposer = new DisposableList
					(
						this.CreateWeakEventListener<VisualStateListenerBase, RoutedEventHandler, RoutedEventArgs>
							((t, o, e) => t.TemplateRootOnUnloaded(), h => tr.Unloaded += h, h => tr.Unloaded -= h),

						this.CreateWeakEventListener<VisualStateListenerBase, RoutedEventHandler, RoutedEventArgs>
							((t, o, e) => t.TemplateRootOnLoaded(), h => tr.Loaded += h, h => tr.Loaded -= h)
					);
				}

				UpdateVisualStateGroup();
			}
		}

		private VisualStateGroup VisualStateGroup
		{
			get => _visualStateGroup;
			set
			{
				if (ReferenceEquals(_visualStateGroup, value))
					return;

				_visualGroupHandlersDisposer = _visualGroupHandlersDisposer.DisposeExchange();

				_visualStateGroup = value;

				if (_visualStateGroup != null)
				{
					var vg = _visualStateGroup;
					_visualGroupHandlersDisposer = new DisposableList
					(
						this.CreateWeakEventListener<VisualStateListenerBase, VisualStateChangedEventArgs>
							((t, o, e) => t.OnVisualStateChanging(), h => vg.CurrentStateChanging += h, h => vg.CurrentStateChanging -= h),
						this.CreateWeakEventListener<VisualStateListenerBase, VisualStateChangedEventArgs>
							((t, o, e) => t.OnVisualStateChanged(e), h => vg.CurrentStateChanged += h, h => vg.CurrentStateChanged -= h)
					);
				}
			}
		}

		#endregion

		#region  Methods

	  public override string ToString()
	  {
	    return $"{VisualStateGroup} - {CurrentStateName}";
	  }

	  private void ControlOnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			UpdateTemplateRoot();
		}

		internal static VisualStateGroup FindVisualStateGroup(FrameworkElement element, string visualStateName)
		{
			return element == null
				? null
				: System.Windows.VisualStateManager.GetVisualStateGroups(element)
					.Cast<VisualStateGroup>()
					.FirstOrDefault(g => g.States.Cast<VisualState>().Select(s => s.Name).Contains(visualStateName));
		}

		private void OnVisualStateChanged(VisualStateChangedEventArgs e)
		{
			Update(e.NewState);
		}

		private void OnVisualStateChanging()
		{
		}

		protected virtual void OnVisualStateEntered()
		{
		}

		protected virtual void OnVisualStateLeaved()
		{
		}

		private void TemplateRootOnLoaded()
		{
			if (_isStateDirty)
				Update();
		}

		private void TemplateRootOnUnloaded()
		{
			_isStateDirty = true;
		}

		private void Update(VisualState state = null)
		{
			CurrentStateName = (state ?? VisualStateGroup.Return(g => g.CurrentState)).Return(s => s.Name);
			_isStateDirty = false;
		}

		private void UpdateTemplateRoot()
		{
			TemplateRoot = _control.GetTemplateRoot<FrameworkElement>();
		}

		private void UpdateVisualStateGroup()
		{
			VisualStateGroup = FindVisualStateGroup(TemplateRoot, _visualStateName);
		}

		#endregion

		#region Interface Implementations

		#region IDisposable

		public void Dispose()
		{
			TemplateRoot = null;
			VisualStateGroup = null;
		}

		#endregion

		#endregion
	}

	internal class VisualStateListener : VisualStateListenerBase
	{
		#region Fields

		public event EventHandler VisualStateLeaved;
		public event EventHandler VisualStateEntered;

		#endregion

		#region Ctors

		public VisualStateListener(Control control, string visualStateName)
			: base(control, visualStateName)
		{
		}

		#endregion

		#region  Methods

		protected override void OnVisualStateEntered()
		{
			VisualStateEntered?.Invoke(this, EventArgs.Empty);
		}

		protected override void OnVisualStateLeaved()
		{
			VisualStateLeaved?.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}

	internal class DelegateVisualStateListener : VisualStateListenerBase
	{
		#region Fields

		private readonly Action _onVisualStateEntered;
		private readonly Action _onVisualStateLeaved;

		#endregion

		#region Ctors

		public DelegateVisualStateListener(Control control, string visualStateName, Action onVisualStateEntered, Action onVisualStateLeaved)
			: base(control, visualStateName)
		{
			_onVisualStateEntered = onVisualStateEntered.ConvertWeak() ?? DummyAction.Instance;
			_onVisualStateLeaved = onVisualStateLeaved.ConvertWeak() ?? DummyAction.Instance;
		}

		#endregion

		#region  Methods

		protected override void OnVisualStateEntered()
		{
			_onVisualStateEntered();
		}

		protected override void OnVisualStateLeaved()
		{
			_onVisualStateLeaved();
		}

		#endregion
	}

	internal static class DelegateVisualStateListenerExtensions
	{
		#region  Methods

		public static VisualStateListenerBase DelegateVisualStateListener(this Control control, string visualStateName, Action onVisualStateEntered,
			Action onVisualStateLeaved)
		{
			return new DelegateVisualStateListener(control, visualStateName, onVisualStateEntered, onVisualStateLeaved);
		}

		#endregion
	}

	internal class WeakFrameworkElement
	{
		#region Fields

		// ReSharper disable once RedundantNameQualifier
		private readonly Core.Weak.WeakReference<FrameworkElement> _frameworkElementReference;

		public event RoutedEventHandler Loaded
		{
			add { Target?.AddHandler(FrameworkElement.LoadedEvent, value, false); }
			// ReSharper disable once ValueParameterNotUsed
			remove { }
		}

		#endregion

		#region Ctors

		public WeakFrameworkElement(FrameworkElement frameworkElement)
		{
			// ReSharper disable once RedundantNameQualifier
			_frameworkElementReference = new Core.Weak.WeakReference<FrameworkElement>(frameworkElement);
		}

		#endregion

		#region Properties

		public FrameworkElement Target => _frameworkElementReference.IsAlive ? _frameworkElementReference.Target : null;

		#endregion
	}
}