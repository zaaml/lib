// <copyright file="ExpressionParameterCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore
{
	public sealed class ExpressionParameterCollection : InheritanceContextDependencyObjectCollection<ExpressionParameter>
	{
		#region Ctors

		internal ExpressionParameterCollection(ExpressionScope expressionScope)
		{
			ExpressionScope = expressionScope;
		}

		#endregion

		#region Properties

		public ExpressionScope ExpressionScope { get; }

		private Dictionary<string, ExpressionParameter> ParametersDictionary { get; } = new Dictionary<string, ExpressionParameter>();

		#endregion

		#region  Methods

		internal ExpressionParameter GetParameter(string name)
		{
			ExpressionParameter parameter;

			return ParametersDictionary.TryGetValue(name, out parameter) ? parameter : null;
		}

		protected override void OnItemAdded(ExpressionParameter parameter)
		{
			base.OnItemAdded(parameter);

			parameter.Collection = this;

			ParametersDictionary.Add(parameter.Name, parameter);

			ExpressionScope.OnParameterAdded(parameter);
		}

		protected override void OnItemRemoved(ExpressionParameter parameter)
		{
			ExpressionScope.OnParameterRemoved(parameter);

			parameter.Collection = null;

			base.OnItemRemoved(parameter);

			ParametersDictionary.Remove(parameter.Name);
		}

		internal void OnParameterNameChanged(ExpressionParameter expressionParameter, string oldName, string newName)
		{
			ParametersDictionary.Remove(oldName);
			ParametersDictionary.Add(newName, expressionParameter);
		}

		#endregion

		public void OnParameterValueChanged(ExpressionParameter expressionParameter, object oldValue, object newValue)
		{
			ExpressionScope.OnParameterValueChanged(expressionParameter, oldValue, newValue);
		}
	}
}