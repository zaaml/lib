// <copyright file="Extension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Animation;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using TriggerCollection = Zaaml.PresentationCore.Interactivity.TriggerCollection;

namespace Zaaml.PresentationCore
{
	public static partial class Extension
	{
		private static readonly DependencyPropertyKey AnimationsPropertyKey = DPM.RegisterAttachedReadOnly<AnimationCollection>
			("AnimationsPrivate", typeof(Extension));

		private static readonly DependencyPropertyKey AssetsPropertyKey = DPM.RegisterAttachedReadOnly<AssetCollection>
			("AssetsPrivate", typeof(Extension));

		private static readonly DependencyPropertyKey BehaviorsPropertyKey = DPM.RegisterAttachedReadOnly<BehaviorCollection>
			("BehaviorsPrivate", typeof(Extension));

		private static readonly DependencyPropertyKey SettersPropertyKey = DPM.RegisterAttachedReadOnly<SetterCollection>
			("SettersPrivate", typeof(Extension), OnSettersPropertyChangedPrivate);

		public static readonly DependencyProperty SettersSourceProperty = DPM.RegisterAttached<SetterCollection>
			("SettersSource", typeof(Extension), OnSettersSourceChanged);

		private static readonly DependencyPropertyKey TriggersPropertyKey = DPM.RegisterAttachedReadOnly<TriggerCollection>
			("TriggersPrivate", typeof(Extension), OnTriggersPropertyChangedPrivate);

		public static readonly DependencyProperty TriggersSourceProperty = DPM.RegisterAttached<TriggerCollection>
			("TriggersSource", typeof(Extension), OnTriggersSourceChanged);

		private static readonly DependencyProperty ActualSkinPrivateProperty = DPM.RegisterAttached<SkinBase>
			("ActualSkinPrivate", typeof(Extension), OnActualSkinPrivateChanged);

		private static readonly DependencyPropertyKey ActualSkinPropertyKey = DPM.RegisterAttachedReadOnly<SkinBase>
			("ActualSkin", typeof(Extension), OnActualSkinChanged);

		private static readonly DependencyPropertyKey ActualClassPropertyKey = DPM.RegisterAttachedReadOnly<ReadOnlyClassList>
			("ActualClass", typeof(Extension));

		private static readonly DependencyPropertyKey ActualClassPrivatePropertyKey = DPM.RegisterAttachedReadOnly<ClassList>
			("ActualClassPrivate", typeof(Extension));

		public static readonly DependencyProperty ActualSkinProperty = ActualSkinPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualClassProperty = ActualClassPropertyKey.DependencyProperty;

		internal static readonly DependencyProperty StyleSkinProperty = DPM.RegisterAttached<SkinBase>
			("StyleSkin", typeof(Extension), OnStyleSkinChanged);

		[TypeConverter(typeof(SkinTypeConverter))]
		public static readonly DependencyProperty SkinProperty = DPM.RegisterAttached<SkinBase>
			("Skin", typeof(Extension), OnSkinChanged);

		[TypeConverter(typeof(ClassListTypeConverter))]
		public static readonly DependencyProperty ClassProperty = DPM.RegisterAttached<ClassList>
			("Class", typeof(Extension), OnClassChanged);

		static Extension()
		{
			PlatformCtor();
		}

		public static ClassList AddClass(DependencyObject dependencyObject, string @class)
		{
			return EnsureActualClass(dependencyObject).AddClass(@class);
		}

		internal static ClassList EnsureActualClass(DependencyObject dependencyObject)
		{
			var actualClassPrivate = (ClassList)dependencyObject.GetValue(ActualClassPrivatePropertyKey.DependencyProperty);

			if (actualClassPrivate == null)
			{
				actualClassPrivate = new ClassList(dependencyObject);

				dependencyObject.SetValue(ActualClassPrivatePropertyKey, actualClassPrivate);
				dependencyObject.SetValue(ActualClassPropertyKey, new ReadOnlyClassList(actualClassPrivate));
			}

			return actualClassPrivate;
		}

		public static ReadOnlyClassList GetActualClass(DependencyObject dependencyObject)
		{
			return (ReadOnlyClassList)dependencyObject.GetValue(ActualClassProperty);
		}

		public static SkinBase GetActualSkin(FrameworkElement element)
		{
			return (SkinBase)element.GetValue(ActualSkinProperty);
		}

		public static AnimationCollection GetAnimations(FrameworkElement element)
		{
			return element.GetValueOrCreate(AnimationsPropertyKey, () => new AnimationCollection(element));
		}

		public static AssetCollection GetAssets(FrameworkElement element)
		{
			return element.GetValueOrCreate(AssetsPropertyKey, () => new AssetCollection(element));
		}

		public static BehaviorCollection GetBehaviors(FrameworkElement element)
		{
			return GetBehaviorsInternal(element, true);
		}

		internal static BehaviorCollection GetBehaviorsInternal(FrameworkElement element, bool create)
		{
			return element.GetValueOrCreateOrDefault(BehaviorsPropertyKey, create, () => new BehaviorCollection(element));
		}

		public static ClassList GetClass(DependencyObject dependencyObject)
		{
			return (ClassList)dependencyObject.GetValue(ClassProperty);
		}

		public static SetterCollection GetSetters(FrameworkElement element)
		{
			return element.GetValueOrCreate(SettersPropertyKey, () => new SetterCollection(element));
		}

		internal static SetterCollection GetSettersInternal(FrameworkElement element, bool create)
		{
			return element.GetValueOrCreateOrDefault(SettersPropertyKey, create, () => new SetterCollection(element));
		}

		public static SetterCollection GetSettersSource(FrameworkElement element)
		{
			return GetSettersInternal(element, false)?.CloneParent;
		}

		public static SkinBase GetSkin(DependencyObject dependencyObject)
		{
			return (SkinBase)dependencyObject.GetValue(SkinProperty);
		}

		private static SkinBase GetStyleSkin(DependencyObject dependencyObject)
		{
			return (SkinBase)dependencyObject.GetValue(StyleSkinProperty);
		}

		public static TriggerCollection GetTriggers(FrameworkElement element)
		{
			return element.GetValueOrCreate(TriggersPropertyKey, () => new TriggerCollection(element));
		}

		internal static TriggerCollection GetTriggersInternal(FrameworkElement element, bool create)
		{
			return element.GetValueOrCreateOrDefault(TriggersPropertyKey, create, () => new TriggerCollection(element));
		}

		public static TriggerCollection GetTriggersSource(FrameworkElement element)
		{
			return GetTriggersInternal(element, false)?.CloneParent;
		}

		private static void OnActualSkinChanged(DependencyObject dependencyObject, SkinBase oldSkin, SkinBase newSkin)
		{
			var frameworkElement = dependencyObject as FrameworkElement;
			var interactivityService = frameworkElement?.GetInteractivityService();

			oldSkin?.OnDetachedInternal(dependencyObject);
			newSkin?.OnAttachedInternal(dependencyObject);

			interactivityService?.OnSkinChanged(oldSkin, newSkin);
		}

		private static void OnActualSkinPrivateChanged(DependencyObject dependencyObject, SkinBase oldSkin, SkinBase newSkin)
		{
			dependencyObject.SetReadOnlyValue(ActualSkinPropertyKey, newSkin);
		}

		private static void OnClassChanged(DependencyObject depObj, ClassList oldClass, ClassList newClass)
		{
			var actualClass = EnsureActualClass(depObj);

			if (oldClass != null)
				actualClass.RemoveClassList(oldClass);

			if (newClass != null)
				actualClass.AddClassList(newClass);
		}

		private static void OnSettersPropertyChangedPrivate(DependencyObject dependencyObject, SetterCollection oldValue, SetterCollection newValue)
		{
			oldValue?.Unload();
			newValue?.Load();
		}

		private static void OnSettersSourceChanged(DependencyObject dependencyObject, SetterCollection oldSetters, SetterCollection newSetters)
		{
			SetSettersSource((FrameworkElement)dependencyObject, newSetters);
		}

		private static void OnSkinChanged(DependencyObject dependencyObject, SkinBase oldSkin, SkinBase newSkin)
		{
			UpdateActualSkin(dependencyObject);
		}

		private static void OnStyleSkinChanged(DependencyObject dependencyObject, SkinBase oldDynamicSkin, SkinBase newDynamicSkin)
		{
			UpdateActualSkin(dependencyObject);
		}

		private static void OnTriggersPropertyChangedPrivate(DependencyObject dependencyObject, TriggerCollection oldValue, TriggerCollection newValue)
		{
			oldValue?.Unload();
			newValue?.Load();
		}

		private static void OnTriggersSourceChanged(DependencyObject dependencyObject, TriggerCollection oldTriggers, TriggerCollection newTriggers)
		{
			SetTriggersSource((FrameworkElement)dependencyObject, newTriggers);
		}

		static partial void PlatformCtor();

		public static ClassList RemoveClass(DependencyObject dependencyObject, string @class)
		{
			return EnsureActualClass(dependencyObject).RemoveClass(@class);
		}

		[TypeConverter(typeof(ClassListTypeConverter))]
		public static void SetClass(DependencyObject dependencyObject, ClassList classList)
		{
			dependencyObject.SetValue(ClassProperty, classList);
		}

		public static void SetSettersSource(FrameworkElement element, SetterCollection setters)
		{
			element.SetReadOnlyValue(SettersPropertyKey, setters?.DeepClone(element));
		}

		[TypeConverter(typeof(SkinTypeConverter))]
		public static void SetSkin(DependencyObject dependencyObject, SkinBase skin)
		{
			dependencyObject.SetValue(SkinProperty, skin);
		}

		public static void SetTriggersSource(FrameworkElement element, TriggerCollection triggers)
		{
			element.SetReadOnlyValue(TriggersPropertyKey, triggers?.DeepClone(element));
		}

		public static ClassList ToggleClass(DependencyObject dependencyObject, string @class)
		{
			return EnsureActualClass(dependencyObject).RemoveClass(@class);
		}

		private static void UpdateActualSkin(DependencyObject dependencyObject)
		{
			var actualSkin = GetSkin(dependencyObject) ?? GetStyleSkin(dependencyObject);

			if (actualSkin is DeferSkin deferSkin)
				dependencyObject.SetBinding(ActualSkinPrivateProperty, new ThemeResourceExtension { Key = deferSkin.Key }.GetBinding(dependencyObject, ActualSkinPrivateProperty));
			else if (actualSkin != null)
				dependencyObject.SetValue(ActualSkinPrivateProperty, actualSkin);
		}
	}
}