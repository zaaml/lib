// <copyright file="SizeLinkedListTest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using NUnit.Framework;
using Zaaml.Core.Collections.Specialized;

namespace Zaaml.Core.Test.Collections.Specialized
{
	[TestFixture]
	public class SizeLinkedListTest
	{
		[Test]
		public void GenericTest()
		{
			var sizeList = new SizeLinkedList();

			sizeList.AddSize(5);
			sizeList.AddSize(10);
			sizeList.AddSize(10);
			sizeList.AddSize(10);

			sizeList[1] = 20;

			sizeList.InsertSize(0, 5);
		}

		[Test]
		public void RemoveSizeTest()
		{
			var sizeList = new SizeLinkedList();

			sizeList.AddSize(5);
			sizeList.AddSize(10);
			sizeList.AddSize(10);
			sizeList.AddSize(5);

			sizeList.RemoveSizeRange(1, 2);
		}

		[Test]
		public void SetSizeTest()
		{
			var sizeList = new SizeLinkedList();

			sizeList.AddSize(5);
			sizeList.AddSize(10);
			sizeList.AddSize(5);
			sizeList.AddSize(10);

			sizeList[2] = 10;
		}
	}
}