// <copyright file="ItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	//internal class ItemGeneratorImpl<T> where T : FrameworkElement, new()
	//{
	//  public void AttachItem(T item, object itemSource)
	//  {
	//    item.DataContext = itemSource;

	//    if (ItemTemplate != null) return;

	//    item.Content = itemSource;
	//  }

	//  public T CreateItem(object itemSource)
	//  {
	//    var itemTemplate = ItemTemplate;
	//    if (itemTemplate == null)
	//      return new T();

	//    var item = itemTemplate.LoadContent() as T;
	//    if (item == null)
	//      throw new InvalidOperationException();

	//    return item;
	//  }

	//  public void DetachItem(T item, object itemSource)
	//  {
	//    if (ReferenceEquals(item.DataContext, itemSource))
	//      item.ClearValue(FrameworkElement.DataContextProperty);

	//    if (ReferenceEquals(item.Content, itemSource))
	//      item.ClearValue(ContentControl.ContentProperty);
	//  }

	//  public void DisposeItem(T item, object itemSource)
	//  {
	//  }
	//}
}