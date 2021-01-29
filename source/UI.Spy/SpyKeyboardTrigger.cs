// <copyright file="SpyKeyboardTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyKeyboardTrigger : SpyTrigger
	{
		public static readonly DependencyProperty ModifierKeysProperty = DPM.Register<ModifierKeys, SpyKeyboardTrigger>
			("ModifierKeys", ModifierKeys.None, d => d.OnModifierKeysPropertyChangedPrivate);

		public static readonly DependencyProperty KeyProperty = DPM.Register<Key?, SpyKeyboardTrigger>
			("Key", null, d => d.OnKeyPropertyChangedPrivate);

		public SpyKeyboardTrigger()
		{
			RenderingObserver = new CompositionRenderingObserver(UpdateState);
		}

		public Key? Key
		{
			get => (Key?) GetValue(KeyProperty);
			set => SetValue(KeyProperty, value);
		}

		public ModifierKeys ModifierKeys
		{
			get => (ModifierKeys) GetValue(ModifierKeysProperty);
			set => SetValue(ModifierKeysProperty, value);
		}

		private CompositionRenderingObserver RenderingObserver { [UsedImplicitly] get; }

		private void OnKeyPropertyChangedPrivate(Key? oldValue, Key? newValue)
		{
			UpdateState();
		}

		private void OnModifierKeysPropertyChangedPrivate(ModifierKeys oldValue, ModifierKeys newValue)
		{
			UpdateState();
		}

		private void UpdateState()
		{
			var modifierKeys = ModifierKeys;
			var key = Key;

			if (modifierKeys == ModifierKeys.None && key == null)
			{
				IsOpen = true;

				return;
			}

			if (Keyboard.Modifiers != modifierKeys && modifierKeys != ModifierKeys.None)
			{
				IsOpen = false;

				return;
			}

			if (key != null && Keyboard.IsKeyDown(key.Value) == false)
			{
				IsOpen = false;

				return;
			}

			IsOpen = true;
		}
	}
}