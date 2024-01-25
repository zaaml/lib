// <copyright file="KeyChordGesture.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
	public class KeyChordGesture : InputGesture
	{
		private int _matchedKeyStrokeIndex;

		public KeyChordGesture()
		{
			KeyStrokes = new KeyStrokeCollection();
		}

		public KeyChordGesture(KeyStroke keyStroke) : this()
		{
			KeyStrokes.Add(keyStroke);
		}

		public KeyChordGesture(KeyStroke keyStroke1, KeyStroke keyStroke2) : this()
		{
			KeyStrokes.Add(keyStroke1);
			KeyStrokes.Add(keyStroke2);
		}

		public KeyChordGesture(params KeyStroke[] keyStrokes) : this()
		{
			foreach (var keyStroke in keyStrokes)
				KeyStrokes.Add(keyStroke);
		}

		private KeyChordGesture(KeyStrokeCollection keyStrokeCollection)
		{
			KeyStrokes = keyStrokeCollection;
		}

		public KeyStrokeCollection KeyStrokes { get; }

		private static Dictionary<string, KeyChordGesture> StaticGestures { get; } = new();

		public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
		{
			if (KeyStrokes.Count == 0)
				return false;

			if (KeyStrokes[_matchedKeyStrokeIndex].Matches(targetElement, inputEventArgs))
				_matchedKeyStrokeIndex++;
			else
				_matchedKeyStrokeIndex = 0;

			if (_matchedKeyStrokeIndex == KeyStrokes.Count)
			{
				_matchedKeyStrokeIndex = 0;

				return true;
			}

			return false;
		}

		public static KeyChordGesture Parse(string keyChordString)
		{
			if (TryParse(keyChordString, out var gesture))
				return gesture;

			throw new InvalidOperationException();
		}

		public static bool TryParse(string keyChordString, out KeyChordGesture gesture)
		{
			if (StaticGestures.TryGetValue(keyChordString, out gesture))
				return true;

			if (KeyStrokeCollection.TryParse(keyChordString, out var keyStrokeCollection) == false)
				return false;

			gesture = new KeyChordGesture(keyStrokeCollection);

			StaticGestures[keyChordString] = gesture;

			return true;
		}
	}
}