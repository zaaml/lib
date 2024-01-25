// <copyright file="KeyStroke.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows.Input;
using Zaaml.Core;

namespace Zaaml.PresentationCore.Input
{
	[TypeConverter(typeof(KeyStrokeConverter))]
	public class KeyStroke : InputGesture
	{
		private Key _key;
		private ModifierKeys _modifier;

		public KeyStroke()
		{
		}

		public KeyStroke(Key key) : this(key, ModifierKeys.None)
		{
		}

		public KeyStroke(Key key, ModifierKeys modifier)
		{
			_key = key;
			_modifier = modifier;
		}

		public Key Key
		{
			get => _key;
			set => _key = value;
		}

		public ModifierKeys Modifiers
		{
			get => _modifier;
			set => _modifier = value;
		}

		public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
		{
			if (inputEventArgs is not KeyEventArgs args)
				return false;

			return Key == args.Key && Modifiers == Keyboard.Modifiers;
		}

		public override string ToString()
		{
			var modifierConverter = new ModifierKeysConverter();
			var keyConverter = new KeyConverter();

			var modStr = _modifier != ModifierKeys.None ? modifierConverter.ConvertToInvariantString(_modifier) + "+" : string.Empty;
			var keyStr = keyConverter.ConvertToInvariantString(_key);

			return modStr + keyStr;
		}

		public static bool TryParse(string str, out KeyStroke keyStroke)
		{
			keyStroke = null;

			try
			{
				var lastPlus = str.LastIndexOf('+');

				if (lastPlus != -1)
				{
					var modifierStr = str.Substring(0, lastPlus);
					var keyStr = str.Substring(lastPlus + 1);

					var modifierConverter = new ModifierKeysConverter();
					var keyConverter = new KeyConverter();

					var key = (Key) keyConverter.ConvertFromInvariantString(keyStr);
					var modifier = (ModifierKeys) modifierConverter.ConvertFromInvariantString(modifierStr);

					keyStroke = new KeyStroke(key, modifier);
					return true;
				}
				else
				{
					var keyConverter = new KeyConverter();
					var key = (Key) keyConverter.ConvertFromInvariantString(str);

					keyStroke = new KeyStroke(key);

					return true;
				}
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			return false;
		}
	}
}