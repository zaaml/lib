//  <copyright file="TextBoxTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

namespace Zaaml.PresentationCore.KnownTemplates
{
  public sealed class TextBoxTemplateContract
  {
#if SILVERLIGHT
    public static readonly string ContentElementName = "ContentElement";
#else
		public static readonly string ContentElementName = "PART_ContentHost";
#endif
  }
}