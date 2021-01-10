// <copyright file="PathBindingBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
#if SILVERLIGHT
using System.ComponentModel;
#else
using System.Globalization;
#endif
using NativeBinding = System.Windows.Data.Binding;


namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public abstract class PathBindingBase : BindingBaseExtension
	{
		#region Fields

		private object _path;

		#endregion

		#region Properties

		public object Path
		{
			get { return _path; }
			set
			{
				_path = value;

#if SILVERLIGHT
				var strPath = _path as string;

				if (strPath == null)
					return;
				
				if (strPath.IndexOf('(') == -1) return;

				_prebuildBinding = new NativeBinding();
				var isupportInitialize = (ISupportInitialize) _prebuildBinding;
				isupportInitialize.BeginInit();
				_prebuildBinding.Path = new PropertyPath(strPath);
				isupportInitialize.EndInit();
#endif
			}
		}

		#endregion

#if !SILVERLIGHT
		private static readonly PropertyPathConverter PathConverter = new PropertyPathConverter();
#endif

		#region  Methods

		protected override void FinalizeXamlInitializationCore(IServiceProvider serviceProvider)
		{
			base.FinalizeXamlInitializationCore(serviceProvider);
			Path = EvaluatePropertyPath(serviceProvider);
		}

		private PropertyPath EvaluatePropertyPath(IServiceProvider serviceProvider)
		{
			if (Path is PropertyPath propertyPath)
				return propertyPath;

			var dpPath = Path as DependencyProperty;

			if (Path is string stringPath)
			{
#if SILVERLIGHT
        return new PropertyPath(stringPath);
#else
				using (var typeDescriptorContext = TypeDescriptorContext.FromServiceProvider(serviceProvider))
					return (PropertyPath) PathConverter.ConvertFrom(typeDescriptorContext, CultureInfo.CurrentCulture, stringPath);
#endif
			}

			return dpPath != null ? new PropertyPath(dpPath) : null;
		}

		protected override NativeBinding GetBindingCore(IServiceProvider serviceProvider)
		{
#if SILVERLIGHT
			if (_prebuildBinding != null)
			{
				if (_isInitialized == false)
				{
          InitSource(_prebuildBinding);
          InitBinding(_prebuildBinding);
					_isInitialized = true;
				}

				return _prebuildBinding;
			}
#endif
			var actualPath = EvaluatePropertyPath(serviceProvider);

			var binding = actualPath != null ? new NativeBinding { Path = actualPath } : new NativeBinding { BindsDirectlyToSource = true };

			InitSource(binding);

			InitBinding(binding);
			return binding;
		}

		protected abstract void InitSource(NativeBinding binding);

		#endregion

#if SILVERLIGHT
		private NativeBinding _prebuildBinding;
		private bool _isInitialized;
#endif
	}
}