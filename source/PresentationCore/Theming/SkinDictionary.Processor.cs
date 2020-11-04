// <copyright file="SkinDictionary.Processor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.Theming
{
	public partial class SkinDictionary
	{
		#region Properties

		private SkinDictionaryProcessorCollection Processors { get; }

		#endregion

		#region  Methods

		private void ApplyProcessors()
		{
			foreach (var processor in Processors)
				processor.ProcessInternal();
		}

		private void FreezeProcessors()
		{
			ApplyProcessors();

			foreach (var skinDictionary in Flatten().Select(kv => kv.Value).OfType<SkinDictionary>())
				skinDictionary.ApplyProcessors();
		}

		#endregion
	}

	public abstract class SkinDictionaryProcessor : DependencyObject
	{
		#region Fields

		private SkinDictionary _skinDictionary;

		#endregion

		#region Properties

		public SkinDictionary SkinDictionary
		{
			get => _skinDictionary;
			internal set
			{
				if (ReferenceEquals(_skinDictionary, value))
					return;

				if (_skinDictionary != null)
					DetachSkinPrivate(_skinDictionary);

				_skinDictionary = value;

				if (_skinDictionary != null)
					AttachSkinPrivate(_skinDictionary);
			}
		}

		#endregion

		#region  Methods

		private void AttachSkinPrivate(SkinDictionary skinDictionary)
		{
		}

		internal SkinDictionaryProcessor Clone()
		{
			var clone = CreateInstance();

			clone.CopyFrom(this);

			return clone;
		}

		protected virtual void CopyFrom(SkinDictionaryProcessor processorSource)
		{
		}

		protected abstract SkinDictionaryProcessor CreateInstance();

		private void DetachSkinPrivate(SkinDictionary skinDictionary)
		{
		}

		protected abstract void ProcessCore();

		internal void ProcessInternal()
		{
			ProcessCore();
		}

		#endregion
	}

	internal sealed class SkinDictionaryProcessorCollection : Collection<SkinDictionaryProcessor>
	{
		#region Ctors

		internal SkinDictionaryProcessorCollection(SkinDictionary skinDictionary)
		{
			SkinDictionary = skinDictionary;
		}

		#endregion

		#region Properties

		public SkinDictionary SkinDictionary { get; }

		#endregion

		#region  Methods

		protected override void ClearItems()
		{
			foreach (var processor in this)
				processor.SkinDictionary = null;

			base.ClearItems();
		}

		protected override void InsertItem(int index, SkinDictionaryProcessor item)
		{
			base.InsertItem(index, item);

			item.SkinDictionary = SkinDictionary;
		}

		protected override void RemoveItem(int index)
		{
			var processor = this[index];

			processor.SkinDictionary = null;

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, SkinDictionaryProcessor item)
		{
			var processor = this[index];

			processor.SkinDictionary = null;

			base.SetItem(index, item);

			processor.SkinDictionary = SkinDictionary;
		}

		#endregion
	}

	public sealed class SolidColorBrushProcessor : SkinDictionaryProcessor
	{
		#region Static Fields and Constants

		private static readonly ExpressionScope Empty = new ExpressionScope();

		#endregion

		#region Fields

		private Func<ExpressionScope, object> _expressionFunc;

		#endregion

		#region Properties

		public string ColorExpression { get; set; }

		#endregion

		#region  Methods

		protected override void CopyFrom(SkinDictionaryProcessor processorSource)
		{
			base.CopyFrom(processorSource);

			var brushProcessor = (SolidColorBrushProcessor) processorSource;

			ColorExpression = brushProcessor.ColorExpression;

			var scope = Expression.GetScope(processorSource);

			Expression.SetScope(this, scope.CloneInternal());
		}

		protected override SkinDictionaryProcessor CreateInstance()
		{
			return new SolidColorBrushProcessor();
		}

		private object GetActualScopeParameterValue(ExpressionParameter parameter)
		{
			var value = parameter.Value;
			var stringValue = value as string;

			if (stringValue == null)
				return value;

			var trimmedValue = stringValue.Trim();

			if (trimmedValue.StartsWith("$(") && trimmedValue.EndsWith(")"))
			{
				var resourceKey = trimmedValue.Substring(2, trimmedValue.Length - 3);
				object actualValue;

				if (SkinDictionary.TryGetValue(resourceKey, out actualValue))
					return actualValue;
			}

			return value;
		}

		protected override void ProcessCore()
		{
			var processorScope = ProcessScope(Expression.GetScope(this));

			try
			{
				_expressionFunc = string.IsNullOrEmpty(ColorExpression) ? ExpressionEngine.FallbackExpressionFunc : ExpressionEngine.Instance.Compile(ColorExpression);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);

				_expressionFunc = ExpressionEngine.FallbackExpressionFunc;
			}

			var processedBrushes = new List<KeyValuePair<string, object>>();

			foreach (var keyValue in SkinDictionary)
			{
				var brush = keyValue.Value as SolidColorBrush;

				if (brush == null)
					continue;

				var brushScope = ProcessScope(Expression.GetScope(brush));

				brushScope.ParentScope = processorScope;

				var evalValue = _expressionFunc(brushScope);

				if (evalValue != null)
					processedBrushes.Add(new KeyValuePair<string, object>(keyValue.Key, new SolidColorBrush(XamlConverter.Convert<Color>(evalValue))));
			}

			foreach (var keyValuePair in processedBrushes)
				SkinDictionary[keyValuePair.Key] = keyValuePair.Value;
		}

		private ExpressionScope ProcessScope(ExpressionScope scope)
		{
			var actualScope = new ExpressionScope();

			if (scope == null)
				return actualScope;

			foreach (var parameter in scope.Parameters)
				actualScope.Parameters.Add(new ExpressionParameter(parameter.Name, GetActualScopeParameterValue(parameter)));

			return actualScope;
		}

		#endregion
	}
}