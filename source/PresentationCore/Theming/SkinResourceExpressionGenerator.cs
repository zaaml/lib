// <copyright file="SkinExpressionResourceGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
{
	public abstract class SkinResourceExpressionGenerator<TResource> : SkinResourceGenerator<TResource>
	{
		protected ExpressionScope ProcessScope(ExpressionScope scope)
		{
			var actualScope = new ExpressionScope();

			if (scope == null)
				return actualScope;

			foreach (var parameter in scope.Parameters)
				actualScope.Parameters.Add(new ExpressionParameter(parameter.Name, GetActualScopeParameterValue(parameter)));

			return actualScope;
		}

		protected object GetActualScopeParameterValue(ExpressionParameter parameter)
		{
			var value = parameter.Value;

			if (value is not string stringValue)
				return value;

			var trimmedValue = stringValue.Trim();

			if (trimmedValue.StartsWith("$(") && trimmedValue.EndsWith(")"))
			{
				var resourceKey = trimmedValue.Substring(2, trimmedValue.Length - 3);

				if (SkinDictionary.TryGetValue(resourceKey, out var actualValue))
					return actualValue;
			}

			return value;
		}
	}
}