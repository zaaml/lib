// <copyright file="DelegateHeaderedContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DelegateHeaderedContentItemGeneratorImpl<TItem, TGenerator> : DefaultHeaderedContentItemGeneratorImpl<TItem, TGenerator> where TItem : FrameworkElement, IHeaderedContentControl, new() where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		#region Fields

		private readonly IHeaderedContentItemsControl _headeredContentControl;

		#endregion

		#region Ctors

		public DelegateHeaderedContentItemGeneratorImpl(IHeaderedContentItemsControl headeredContentControl)
			: base(headeredContentControl.ItemHeaderTemplate, headeredContentControl.ItemHeaderTemplateSelector, headeredContentControl.ItemHeaderStringFormat, headeredContentControl.ItemContentTemplate, headeredContentControl.ItemContentTemplateSelector, headeredContentControl.ItemContentStringFormat)
		{
			_headeredContentControl = headeredContentControl;
		}

		#endregion

		#region  Methods

		public void OnItemContentStringFormatChanged()
		{
			ItemContentStringFormat = _headeredContentControl.ItemContentStringFormat;
		}

		public void OnItemContentTemplateChanged()
		{
			ItemContentTemplate = _headeredContentControl.ItemContentTemplate;
		}

		public void OnItemContentTemplateSelectorChanged()
		{
			ItemContentTemplateSelector = _headeredContentControl.ItemContentTemplateSelector;
		}

		public void OnItemHeaderStringFormatChanged()
		{
			ItemHeaderStringFormat = _headeredContentControl.ItemHeaderStringFormat;
		}

		public void OnItemHeaderTemplateChanged()
		{
			ItemHeaderTemplate = _headeredContentControl.ItemHeaderTemplate;
		}

		public void OnItemHeaderTemplateSelectorChanged()
		{
			ItemHeaderTemplateSelector = _headeredContentControl.ItemHeaderTemplateSelector;
		}

		#endregion
	}
}