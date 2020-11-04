// <copyright file="StaticExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>


#if SILVERLIGHT
using System;
using System.Reflection;
using System.Windows.Markup;

#endif

namespace Zaaml.PresentationCore.MarkupExtensions
{
#if SILVERLIGHT
	public class StaticExtension : MarkupExtensionBase
	{
  #region Static Fields and Constants

		private static readonly char[] Separator = {'.'};

  #endregion

  #region Properties

		public string Member { get; set; }

  #endregion

  #region  Methods

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var typeResolver = (IXamlTypeResolver) serviceProvider.GetService(typeof(IXamlTypeResolver));

			if (typeResolver == null)
				return null;

			var dotSplitted = Member.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			if (dotSplitted.Length != 2)
				return null;

			var type = typeResolver.Resolve(dotSplitted[0]);
			var memberName = dotSplitted[1];

			var propInfo = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			if (propInfo != null)
				return propInfo.GetValue(null, null);

			var fieldInfo = type.GetField(memberName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			return fieldInfo?.GetValue(null);
		}

  #endregion
	}
#else
  public class StaticExtension : System.Windows.Markup.StaticExtension
  {
  }
#endif
}