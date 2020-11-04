// <copyright file="ValidationUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.PresentationCore.Utils
{
  public static class ValidationUtils
  {
    #region  Methods

    public static void ClearValidationError(FrameworkElement frameworkElement)
    {
      CustomValidationError.ClearValidationErrorInternal(frameworkElement);
    }

    public static void SetValidationError(FrameworkElement frameworkElement, string message)
    {
      CustomValidationError.SetValidationErrorInternal(frameworkElement, message);
    }

    public static bool HasValidationError(FrameworkElement frameworkElement)
    {
	    return CustomValidationError.HasValidationErrorInternal(frameworkElement);
    }

    #endregion
  }

	public class CustomValidationError : IDataErrorInfo
	{
		#region Static Fields and Constants

		private static readonly DependencyProperty ValidationProperty = DPM.RegisterAttached<object, CustomValidationError>
			("Validation");

		#endregion

		#region Ctors

		private CustomValidationError(string errorMessage)
		{
			ErrorMessage = errorMessage;
		}

		#endregion

		#region Properties

		// ReSharper disable once MemberCanBePrivate.Local
		public string ErrorMessage { get; }

		#endregion

		#region  Methods

		internal static void ClearValidationErrorInternal(FrameworkElement frameworkElement)
		{
			frameworkElement.ClearValue(ValidationProperty);
		}

		internal static void SetValidationErrorInternal(FrameworkElement frameworkElement, string message)
		{
			var binding = new Binding
			{
				Path = new PropertyPath("ErrorMessage"),
				ValidatesOnDataErrors = true,
				Source = new CustomValidationError(message)
			};

			frameworkElement.SetBinding(ValidationProperty, binding);
		}

		internal static bool HasValidationErrorInternal(FrameworkElement frameworkElement)
		{
			return frameworkElement.HasLocalValue(ValidationProperty);
		}

		#endregion

		#region Interface Implementations

		#region IDataErrorInfo

		string IDataErrorInfo.this[string columnName] => ErrorMessage;

		string IDataErrorInfo.Error => ErrorMessage;

		#endregion

		#endregion
	}
}