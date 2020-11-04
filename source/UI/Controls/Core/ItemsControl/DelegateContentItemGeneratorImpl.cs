// <copyright file="DelegateContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DelegateContentItemGeneratorImpl<TItem, TGenerator> : DefaultContentItemGeneratorImpl<TItem, TGenerator> where TItem : FrameworkElement, IContentControl, new() where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		#region Fields

		private readonly IContentItemsControl _contentControl;

		#endregion

		#region Ctors

		public DelegateContentItemGeneratorImpl(IContentItemsControl contentControl)
			: base(contentControl.ItemContentTemplate, contentControl.ItemContentTemplateSelector, contentControl.ItemContentStringFormat)
		{
			_contentControl = contentControl;
		}

		#endregion

		#region  Methods

		public void OnItemContentStringFormatChanged()
		{
			ItemContentStringFormat = _contentControl.ItemContentStringFormat;
		}

		public void OnItemContentTemplateChanged()
		{
			ItemContentTemplate = _contentControl.ItemContentTemplate;
		}

		public void OnItemContentTemplateSelectorChanged()
		{
			ItemContentTemplateSelector = _contentControl.ItemContentTemplateSelector;
		}

		#endregion
	}
}