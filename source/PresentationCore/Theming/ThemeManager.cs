// <copyright file="ThemeManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Reflection;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Theming
{
	public static partial class ThemeManager
	{
		private static ThemeManagerBehavior _behavior;
		private static Theme _applicationTheme;
		private static readonly WeakLinkedList<IThemeChangeListener> ThemeChangeListeners = new WeakLinkedList<IThemeChangeListener>();
		private static readonly ResourceDictionary ApplicationResourceDictionary = new ResourceDictionary();
		private static readonly WeakLinkedList<ResourceDictionary> ResourceDictionaries = new WeakLinkedList<ResourceDictionary>();
		private static readonly Dictionary<string, ThemeResourceReference> ThemeResourceReferences = new Dictionary<string, ThemeResourceReference>();

		static ThemeManager()
		{
			RuntimeHelpers.RunClassConstructor(typeof(FrameCounter).TypeHandle);

			GCObserver = new GarbageCollectorObserver();
			GCObserver.GarbageCollected += (sender, args) => CleanUpResourceInheritanceParentImpl();

			CompositionTarget.Rendering += CompositionTargetOnRendering;

			// Activate mouse service
			// ReSharper disable once UnusedVariable
			var hostPoint = MouseInternal.ScreenPosition;
		}

		internal static ThemeManagerBehavior ActualBehavior => Behavior ?? DefaultThemeManagerBehavior.Instance;

		internal static Theme ActualTheme => ApplicationThemeCore;

		public static Theme ApplicationTheme
		{
			get => IsDefaultThemeApplied ? null : ApplicationThemeCore;
			set => ApplicationThemeCore = value;
		}

		private static Theme ApplicationThemeCore
		{
			get => _applicationTheme;
			set
			{
				var isInDesignMode = PresentationCoreUtils.IsInDesignMode;

				PresentationCoreUtils.LaunchDebugger();

				var oldTheme = _applicationTheme;
				var newTheme = value ?? DefaultTheme.Instance;

				try
				{
					if (ReferenceEquals(_applicationTheme, newTheme))
						return;

					IsApplicationThemeChanging = true;

					if (isInDesignMode == false)
					{
						if (IsInitializedWithTheme && newTheme?.IsStatic == true)
							throw new InvalidOperationException("Static theme can not be set after ThemeManager has already been initialized with Theme");

						if (IsStatic)
							throw new InvalidOperationException("Static theme can not be changed.");
					}

					IsStatic = isInDesignMode == false && newTheme?.IsStatic == true;
					IsInitializedWithTheme = ReferenceEquals(DefaultTheme.Instance, value) == false;

					if (_applicationTheme is SkinnedTheme currentSkinnedTheme && newTheme is SkinnedTheme newSkinnedTheme && currentSkinnedTheme.BaseTheme == newSkinnedTheme.BaseTheme)
					{
						SkinnedTheme.ChangeSkin(currentSkinnedTheme, newSkinnedTheme);

						foreach (var themeResourceReference in ThemeResourceReferences.Values)
						{
							themeResourceReference.Unbind();
							newSkinnedTheme.BindThemeResource(themeResourceReference);
						}

						_applicationTheme = newTheme;
					}
					else
					{
						if (_applicationTheme != null)
						{
							_applicationTheme.IsApplied = false;

							foreach (var themeResourceReference in ThemeResourceReferences.Values)
								themeResourceReference.Unbind();

							ApplicationResourceDictionary.MergedDictionaries.Remove(_applicationTheme.ThemeResourceDictionary);

							foreach (var resourceDictionary in ResourceDictionaries)
								resourceDictionary.MergedDictionaries.Clear();
						}

						_applicationTheme = newTheme;

						if (_applicationTheme != null)
						{
							_applicationTheme.IsApplied = true;

							foreach (var themeResourceReference in ThemeResourceReferences.Values)
								_applicationTheme.BindThemeResource(themeResourceReference);

							ApplicationResourceDictionary.MergedDictionaries.Add(_applicationTheme.ThemeResourceDictionary);

							foreach (var resourceDictionary in ResourceDictionaries)
								resourceDictionary.MergedDictionaries.Add(_applicationTheme.CreateThemeResourceDictionary());
						}
					}

					ActivateApplicationTheme();

					OnCurrentThemeChanged();
				}
				finally
				{
					if (ReferenceEquals(oldTheme, newTheme) == false)
						foreach (var listener in ThemeChangeListeners)
							listener.OnThemeChanged(oldTheme, newTheme);

					Instance.OnThemeChanged(oldTheme, newTheme);

					IsApplicationThemeChanging = false;

					if (ReferenceEquals(oldTheme, newTheme) == false)
						foreach (var listener in TempListeners)
							listener.OnThemeChanged(oldTheme, newTheme);

					TempListeners.Clear();
				}
			}
		}

		public static ThemeManagerBehavior Behavior
		{
			get => _behavior;
			set
			{
				if (IsInitializedWithTheme)
					throw new InvalidOperationException("ThemeManager behavior can not be changed after ThemeManager has already been initialized with Theme");

				if (ReferenceEquals(_behavior, value))
					return;

				_behavior = value;

				ThemeStyleBinder.OnThemeManagerStrategyChanged();
			}
		}

		private static GarbageCollectorObserver GCObserver { get; }

		public static ThemeManagerInstance Instance { get; } = new ThemeManagerInstance();

		internal static bool IsApplicationThemeChanging { get; private set; }

		private static bool IsAutoActivationEnabled => ActualBehavior.ActivationMode == ThemeManagerActivationMode.Auto;

		internal static bool IsDefaultThemeApplied => ReferenceEquals(DefaultTheme.Instance, ApplicationThemeCore);

		private static bool IsInitializedWithTheme { get; set; }

		internal static bool IsStatic { get; private set; }

		internal static WeakLinkedList<IThemeChangeListener> TempListeners { get; } = new WeakLinkedList<IThemeChangeListener>();

		private static void ActivateApplicationTheme()
		{
			if (IsAutoActivationEnabled == false)
				return;

			PlatformActivateApplicationTheme();
		}

		internal static void AddTempThemeChangedListener(IThemeChangeListener listener)
		{
			if (IsStatic == false)
				TempListeners.Add(listener);
		}

		internal static void AddThemeChangedListener(IThemeChangeListener listener)
		{
			if (IsStatic == false)
				ThemeChangeListeners.Add(listener);
		}

		public static void AssignThemeResource(DependencyObject dependencyObject, DependencyProperty property, string themeResourceKey)
		{
			using var serviceProvider = TargetServiceProvider.GetServiceProvider(dependencyObject, property);

			var value = new ThemeResourceExtension {Key = themeResourceKey}.ProvideValue(serviceProvider);

			if (value is Binding binding)
				dependencyObject.SetBinding(property, binding);
			else
				dependencyObject.SetValue(property, value);
		}

		public static void AssignThemeResource(DependencyObject dependencyObject, DependencyProperty property, ThemeKeyword themeKeyword)
		{
			AssignThemeResource(dependencyObject, property, GetKeyFromKeyword(themeKeyword));
		}

		private static void CleanUpResourceInheritanceParentImpl()
		{
			foreach (var value in ThemeResourceReferences.Values.Select(r => r.Value).OfType<DependencyObject>())
			{
				var brush = value as Brush;
#if !SILVERLIGHT
				if (brush != null && brush.IsFrozen)
					continue;
#endif
				brush.ResetInheritanceParent();
			}
		}

		private static void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
		{
			ActivateApplicationTheme();
		}

		internal static ResourceDictionary CreateThemeResourceDictionary()
		{
			var themeResourceDictionary = new ResourceDictionary();

			ResourceDictionaries.Add(themeResourceDictionary);

			if (_applicationTheme != null)
				themeResourceDictionary.MergedDictionaries.Add(_applicationTheme.CreateThemeResourceDictionary());

			return themeResourceDictionary;
		}

		private static ThemeResourceReference CreateThemeResourceReference(string key)
		{
			var themeResourceReference = new ThemeResourceReference(key);

			ApplicationThemeCore?.BindThemeResource(themeResourceReference);

			return themeResourceReference;
		}

		internal static bool EnableThemeStyle(Type elementType, Type styleType)
		{
			var currentElementType = elementType;

			while (currentElementType != null && currentElementType != typeof(FrameworkElement))
			{
				var assembly = currentElementType.Assembly;

				var behaviorAttribute = assembly.GetAttribute<ThemeManagerBehaviorAttribute>();

				if (behaviorAttribute != null)
					return behaviorAttribute.EnableThemeStyleInt(elementType, styleType);

				currentElementType = currentElementType.BaseType;

				while (currentElementType != null && currentElementType != typeof(FrameworkElement) && currentElementType.Assembly == assembly)
					currentElementType = currentElementType.BaseType;
			}

			return ActualBehavior.EnableThemeStyle(elementType, styleType);
		}

		internal static void EnsureApplicationTheme()
		{
			if (ApplicationThemeCore == null)
				ApplicationTheme = DefaultTheme.Instance;

			ActivateApplicationTheme();
		}

		internal static string GetKeyFromKeyword(ThemeKeyword keyword)
		{
			return keyword == ThemeKeyword.Undefined ? null : string.Concat("Zaaml.ThemeManager.Keywords.", keyword.ToString());
		}

		internal static ThemeResourceReference GetThemeReference(string key, bool create)
		{
			return GetThemeReferenceImpl(key, create);
		}

		internal static ThemeResourceReference GetThemeReference(string key)
		{
			return GetThemeReferenceImpl(key, true);
		}

		internal static ThemeResourceReference GetThemeReference(ThemeKeyword keyword)
		{
			return GetThemeReferenceImpl(GetKeyFromKeyword(keyword), true);
		}

		private static ThemeResourceReference GetThemeReferenceImpl(string key, bool create)
		{
			return ThemeResourceReferences.GetValueOrCreateOrDefault(key, create, CreateThemeResourceReference);
		}

		internal static ThemeManagerStyle GetThemeStyle(Type targetType)
		{
			return ThemeStyleBinder.Instance.GetThemeStyle(targetType);
		}

		private static void OnCurrentThemeChanged()
		{
			CurrentThemeChanged?.Invoke(null, EventArgs.Empty);
		}

		static partial void PlatformActivateApplicationTheme();

		internal static void RemoveThemeChangedListener(IThemeChangeListener listener)
		{
			if (IsStatic == false)
				ThemeChangeListeners.Remove(listener);
		}

		public static event EventHandler CurrentThemeChanged;
	}

	internal interface IThemeChangeListener
	{
		void OnThemeChanged(Theme oldTheme, Theme newTheme);
	}

	internal class DesignTimeThemeManagerBehavior
	{
		private ThemeManagerBehavior _behavior;

		public ThemeManagerBehavior Behavior
		{
			get => _behavior;
			set
			{
				if (PresentationCoreUtils.IsInDesignMode == false)
					return;

				if (ReferenceEquals(_behavior, value))
					return;

				_behavior = value;

				ThemeManager.Behavior = value;
			}
		}
	}

	public sealed class ThemeManagerInstance : INotifyPropertyChanged
	{
		public Theme ApplicationTheme
		{
			get => ThemeManager.ApplicationTheme;
			set => ThemeManager.ApplicationTheme = value;
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		internal void OnThemeChanged(Theme oldTheme, Theme newTheme)
		{
			OnPropertyChanged(nameof(ApplicationTheme));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}