// <copyright file="ClassList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore
{
	[TypeConverter(typeof(ClassListTypeConverter))]
	public sealed class ClassList
	{
		private static readonly char[] Separators = [' '];
		private readonly SortedDictionary<string, int> _classValues = new(StringComparer.OrdinalIgnoreCase);
		private readonly DependencyObject _owner;
		private string _classListString;

		public ClassList()
		{
		}

		public ClassList(string classListString)
		{
			AddClass(classListString);
		}

		public ClassList(DependencyObject owner)
		{
			_owner = owner;
		}

		public IEnumerable<string> Classes
		{
			get
			{
				foreach (var kv in _classValues)
				{
					if (kv.Value > 0)
						yield return kv.Key;
				}
			}
		}

		public string ClassListString => _classListString ??= BuildClassListString();

		public ClassList AddClass(string @class)
		{
			if (NeedParse(@class))
				foreach (var singleClass in ParseClass(@class))
					AddSingleClass(singleClass);
			else
				AddSingleClass(@class);

			OnClassChanged();

			return this;
		}

		internal void AddClassList(ClassList classList)
		{
			foreach (var @class in classList.Classes)
				AddSingleClass(@class);

			OnClassChanged();
		}

		private void AddSingleClass(string @class)
		{
			if (string.IsNullOrWhiteSpace(@class))
				return;

			_classListString = null;

			if (_classValues.TryGetValue(@class, out var count) == false)
				_classValues.Add(@class, 1);
			else
				_classValues[@class] = count + 1;
		}

		private string BuildClassListString()
		{
			return string.Join(" ", Classes);
		}

		public bool HasClass(string @class)
		{
			if (NeedParse(@class))
			{
				foreach (var singleClass in ParseClass(@class))
				{
					if (HasSingleClass(singleClass) == false)
						return false;
				}

				return true;
			}

			return HasSingleClass(@class);
		}

		private bool HasSingleClass(string @class)
		{
			if (string.IsNullOrWhiteSpace(@class))
				return true;

			return _classValues.TryGetValue(@class, out var count) && count > 0;
		}

		private static bool NeedParse(string @class)
		{
			return @class.IndexOf(' ') != -1;
		}

		private void OnClassChanged()
		{
			var frameworkElement = _owner as FrameworkElement;
			var interactivityService = frameworkElement?.GetInteractivityService();

			interactivityService?.OnClassChanged();
		}

		private static string[] ParseClass(string @class)
		{
			return @class.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
		}

		public ClassList RemoveClass(string @class)
		{
			if (NeedParse(@class))
				foreach (var singleClass in ParseClass(@class))
					RemoveSingleClass(singleClass);
			else
				RemoveSingleClass(@class);

			OnClassChanged();

			return this;
		}

		internal void RemoveClassList(ClassList classList)
		{
			foreach (var @class in classList.Classes)
				RemoveSingleClass(@class);

			OnClassChanged();
		}

		private void RemoveSingleClass(string @class)
		{
			if (string.IsNullOrWhiteSpace(@class))
				return;

			_classListString = null;

			if (_classValues.TryGetValue(@class, out var count) == false)
				return;

			if (count == 1)
				_classValues.Remove(@class);
			else
				_classValues[@class] = count - 1;
		}

		public ClassList ToggleClass(string @class)
		{
			if (NeedParse(@class))
				foreach (var singleClass in ParseClass(@class))
					ToggleSingleClass(singleClass);
			else
				ToggleSingleClass(@class);

			OnClassChanged();

			return this;
		}

		private void ToggleSingleClass(string @class)
		{
			if (string.IsNullOrWhiteSpace(@class))
				return;

			_classListString = null;

			if (HasClass(@class))
				RemoveClass(@class);
			else
				AddClass(@class);
		}
	}
}