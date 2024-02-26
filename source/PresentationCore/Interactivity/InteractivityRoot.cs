// <copyright file="InteractivityRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
	internal abstract partial class InteractivityRoot : IInteractivityRoot
	{
		private WeakReference _xamlRootWeak;

		internal InteractivityRoot(InteractivityService interactivityService)
		{
			InteractivityService = interactivityService;
		}

		public virtual DependencyObject FindName(string name)
		{
			return null;
		}

		protected abstract void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName);

		protected virtual void OnXamlRootChanged(object oldXamlRoot, object newXamlRoot)
		{
		}

		public static void UpdateClass(InteractivityCollection collection)
		{
			collection?.WalkTree(UpdateClassVisitor.Instance);
		}

		public virtual void UpdateClass()
		{
		}

		public static void UpdateSkin(InteractivityCollection collection, SkinBase skin)
		{
			collection?.WalkTree(skin ?? UpdateNullSkinVisitor.Instance);
		}

		public virtual void UpdateSkin(SkinBase skin)
		{
		}

		public void UpdateThemeResources(InteractivityCollection collection)
		{
			collection?.WalkTree(UpdateThemeResourcesVisitor.Instance);
		}

		public virtual void UpdateThemeResources()
		{
		}

		IInteractivityObject IInteractivityObject.Parent => null;

		void IInteractivityObject.OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
		{
			OnDescendantApiPropertyChanged(descendants, propertyName);
		}

		public InteractivityService InteractivityService { get; }

		public FrameworkElement InteractivityTarget => InteractivityService.Target;

		public virtual object GetService(Type serviceType)
		{
			if (serviceType == typeof(IVisualStateObserver))
			{
				EnsureVisualStateObserver();

				return this;
			}

			return null;
		}

		public object XamlRoot
		{
			get => _xamlRootWeak?.Target;
			set
			{
				var root = XamlRoot;
				if (ReferenceEquals(root, value))
					return;

				_xamlRootWeak = value != null ? new WeakReference(value) : null;

				OnXamlRootChanged(root, value);
			}
		}

		public object ActualXamlRoot => XamlRoot;

		private class UpdateThemeResourcesVisitor : IInteractivityVisitor
		{
			public static readonly IInteractivityVisitor Instance = new UpdateThemeResourcesVisitor();

			private UpdateThemeResourcesVisitor()
			{
			}

			public void Visit(InteractivityObject interactivityObject)
			{
				var setter = interactivityObject as Setter;

				setter?.UpdateThemeResources();
			}
		}

		private class UpdateNullSkinVisitor : IInteractivityVisitor
		{
			public static readonly IInteractivityVisitor Instance = new UpdateNullSkinVisitor();

			private UpdateNullSkinVisitor()
			{
			}

			public void Visit(InteractivityObject interactivityObject)
			{
				var setter = interactivityObject as Setter;

				setter?.UpdateSkin(null);
			}
		}

		private class UpdateClassVisitor : IInteractivityVisitor
		{
			public static readonly IInteractivityVisitor Instance = new UpdateClassVisitor();

			private UpdateClassVisitor()
			{
			}

			public void Visit(InteractivityObject interactivityObject)
			{
				if (interactivityObject is ClassTrigger classTrigger)
					classTrigger.OnClassChangedInternal();
				else if (interactivityObject is Setter setter)
					setter.OnClassChangedInternal();

			}
		}
	}
}