// <copyright file="ThemeStyleBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.Core.Reflection;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.PresentationCore.Theming
{
	internal class ThemeStyleBinder
	{
		private static readonly Lazy<ThemeStyleBinder> LazyInstance = new Lazy<ThemeStyleBinder>(() => new ThemeStyleBinder());
		private static readonly Type FrameworkElementType = typeof(FrameworkElement);

		private readonly AppDomainObserver _appDomainObserver;
		private readonly HashSet<Type> _attachedTypes = new HashSet<Type>();
		private readonly HashSet<Type> _themeBoundTypes = new HashSet<Type>();
		private readonly Dictionary<Type, ThemeManagerStyle> _themeManagerStyles = new Dictionary<Type, ThemeManagerStyle>();
		private readonly WeakLinkedList<ResourceDictionary> _themeResourceDictionaries = new WeakLinkedList<ResourceDictionary>();

		private ThemeStyleBinder()
		{
			_appDomainObserver = new AppDomainObserver(LookupFrameworkElementTypes);
			CompositionTarget.Rendering += OnCompositionTargetOnRendering;
		}

		public static ThemeStyleBinder Instance
		{
			get
			{
				var instance = LazyInstance.Value;

				instance.UpdateDomainObserver();

				return instance;
			}
		}

		public void AttachResourceDictionary(ResourceDictionary resourceDictionary)
		{
			AttachStyleTypes(resourceDictionary, _attachedTypes);
			_themeResourceDictionaries.Add(resourceDictionary);
		}

		private void AttachStyleTypes(ResourceDictionary resourceDictionary, IEnumerable<Type> types)
		{
			foreach (var style in types.Select(GetThemeStyle))
				SetResourceStyle(resourceDictionary, style);
		}

		private void AttachStyleTypes(HashSet<Type> types)
		{
			foreach (var resourceDictionary in _themeResourceDictionaries)
				AttachStyleTypes(resourceDictionary, types);
		}

		public void BindThemeStyles(IEnumerable<ThemeStyle> themeStyles)
		{
			BindThemeStylesImpl(themeStyles);
		}

		private void BindThemeStylesImpl(IEnumerable<ThemeStyle> themeStyles)
		{
			foreach (var themeStyle in themeStyles)
			{
				GetThemeStyle(themeStyle.TargetType).BindThemeManagerStyle(themeStyle);
				_themeBoundTypes.Add(themeStyle.TargetType);
			}

			UpdateAttachedTypes();
		}

		private ThemeManagerStyle CreateThemeStyle(Type targetType)
		{
			var themeStyle = new ThemeManagerStyle {TargetType = targetType};

			_themeManagerStyles[targetType] = themeStyle;

			themeStyle.StyleService.NativeStyleChanged += OnNativeStyleChanged;

			return themeStyle;
		}

		public void DetachResourceDictionary(ResourceDictionary resourceDictionary)
		{
			_themeResourceDictionaries.Remove(resourceDictionary);
			DetachStyleTypes(resourceDictionary, _attachedTypes);
		}

		private static void DetachStyleTypes(ResourceDictionary resourceDictionary, IEnumerable<Type> types)
		{
			foreach (var type in types)
				resourceDictionary.Remove(type);
		}

		private void DetachStyleTypes(HashSet<Type> types)
		{
			foreach (var resourceDictionary in _themeResourceDictionaries)
				DetachStyleTypes(resourceDictionary, types);
		}

		public ThemeManagerStyle GetThemeStyle(Type targetType)
		{
			var themeStyle = _themeManagerStyles.GetValueOrDefault(targetType);

			if (themeStyle != null)
				return themeStyle;

			themeStyle = CreateThemeStyle(targetType);

			var rootType = DefaultStyleKeyHelper.GetDefaultStyleKeyType(targetType);
			var current = themeStyle;

			while (targetType != null && targetType != rootType)
			{
				targetType = targetType.BaseType;
				rootType = DefaultStyleKeyHelper.GetDefaultStyleKeyType(targetType);

				var style = _themeManagerStyles.GetValueOrDefault(targetType);

				if (style != null)
				{
					current.BindThemeManagerStyle(style);

					break;
				}

				style = CreateThemeStyle(targetType);
				current.BindThemeManagerStyle(style);
				current = style;
			}

			return themeStyle;
		}

		private void LookupFrameworkElementTypes(IEnumerable<Assembly> assemblies)
		{
			foreach (var assembly in assemblies)
			{
#if !SILVERLIGHT
				var referencedAssemblies = assembly.GetReferencedAssemblies();
				if (ShouldProcess(assembly.GetName()) == false && referencedAssemblies.Any(ShouldProcess) == false)
					continue;
#endif

				foreach (var type in assembly.GetLoadableTypes().Where(SupportStyleType))
					GetThemeStyle(type);
			}
		}

		private void OnCompositionTargetOnRendering(object sender, EventArgs args)
		{
			_appDomainObserver.Update();
		}

		private void OnNativeStyleChanged(object o, EventArgs eventArgs)
		{
			var styleService = (StyleService) o;
			var themeStyle = (ThemeManagerStyle) styleService.Style;
			var targetType = themeStyle.TargetType;

			if (_attachedTypes.Contains(targetType) == false)
				return;

			foreach (var resourceDictionary in _themeResourceDictionaries)
				SetResourceStyle(resourceDictionary, themeStyle);
		}

		public static void OnThemeManagerStrategyChanged()
		{
			Instance.UpdateAttachedTypes();
		}

		public void RebindThemeStyles(IEnumerable<ThemeStyle> themeStyles)
		{
			foreach (var themeStyle in themeStyles.Where(s => _themeBoundTypes.Contains(s.TargetType)))
				GetThemeStyle(themeStyle.TargetType).BindThemeManagerStyle(themeStyle);
		}

		private static void SetResourceStyle(ResourceDictionary resourceDictionary, ThemeManagerStyle style)
		{
			if (resourceDictionary.Contains(style.TargetType))
				resourceDictionary.Remove(style.TargetType);

			resourceDictionary.Add(style.TargetType, style.NativeStyle);
		}

#if !SILVERLIGHT
		private static bool ShouldProcess(AssemblyName assembly)
		{
			return string.Equals(assembly.FullName, FrameworkElementAssembly.FullName, StringComparison.OrdinalIgnoreCase);
		}
#endif

		private static bool SupportStyleType(Type type)
		{
#if SILVERLIGHT
      return type.IsSubclassOf(FrameworkElementType);
#else
			return type.IsSubclassOf(FrameworkElementType) || type.IsSubclassOf(FrameworkContentElementType);
#endif
		}

		public void UnbindThemeStyles(IEnumerable<ThemeStyle> themeStyles)
		{
			UnbindThemeStylesImpl(themeStyles);
		}

		private void UnbindThemeStylesImpl(IEnumerable<ThemeStyle> themeStyles)
		{
			foreach (var themeStyle in themeStyles)
			{
				if (themeStyle.TargetType != FrameworkElementType)
					GetThemeStyle(themeStyle.TargetType).BindThemeManagerStyle(GetThemeStyle(themeStyle.TargetType.BaseType));

				_themeBoundTypes.Remove(themeStyle.TargetType);
			}

			UpdateAttachedTypes();
		}

		private void UpdateAttachedTypes()
		{
			var currentAttachedTypes = new HashSet<Type>();

			foreach (var style in _themeManagerStyles.Values)
			{
				var isExplicit = _themeBoundTypes.Contains(style.TargetType);
				var implicitStyle = (StyleBase) style;

				if (isExplicit == false)
				{
					implicitStyle = style.EnumerateBaseStyles().FirstOrDefault(s => _themeBoundTypes.Contains(s.TargetType));
					if (implicitStyle == null)
						continue;
				}

				if (ThemeManager.EnableThemeStyle(style.TargetType, implicitStyle.TargetType))
					currentAttachedTypes.Add(style.TargetType);
			}

			var temp = currentAttachedTypes.ToArray();

			_attachedTypes.ExceptWith(currentAttachedTypes);

			DetachStyleTypes(_attachedTypes);

			currentAttachedTypes.ExceptWith(_attachedTypes);

			AttachStyleTypes(currentAttachedTypes);

			_attachedTypes.Clear();
			_attachedTypes.UnionWith(temp);
		}

		private void UpdateDomainObserver()
		{
			_appDomainObserver.Update();
		}
#if !SILVERLIGHT
		private static readonly AssemblyName FrameworkElementAssembly = new AssemblyName(FrameworkElementType.Assembly.FullName);
		private static readonly Type FrameworkContentElementType = typeof(FrameworkContentElement);
#endif
	}
}