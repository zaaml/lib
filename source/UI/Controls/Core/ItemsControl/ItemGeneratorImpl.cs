// <copyright file="ItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	//internal class ItemGeneratorImpl<T> where T : FrameworkElement, new()
	//{
	//  public void AttachItem(T item, object source)
	//  {
	//    item.DataContext = source;

	//    if (ItemTemplate != null) return;

	//    item.Content = source;
	//  }

	//  public T CreateItem(object source)
	//  {
	//    var itemTemplate = ItemTemplate;
	//    if (itemTemplate == null)
	//      return new T();

	//    var item = itemTemplate.LoadContent() as T;
	//    if (item == null)
	//      throw new InvalidOperationException();

	//    return item;
	//  }

	//  public void DetachItem(T item, object source)
	//  {
	//    if (ReferenceEquals(item.DataContext, source))
	//      item.ClearValue(FrameworkElement.DataContextProperty);

	//    if (ReferenceEquals(item.Content, source))
	//      item.ClearValue(ContentControl.ContentProperty);
	//  }

	//  public void DisposeItem(T item, object source)
	//  {
	//  }
	//}
}