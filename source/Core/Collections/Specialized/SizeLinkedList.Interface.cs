// <copyright file="SizeLinkedList.Interface.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Collections.Specialized
{
	internal partial class SizeLinkedList
	{
		public double AverageSize => Count > 0 ? Size / Count : 0;
		public long Count { get; private set; }

		public double this[long index]
		{
			get
			{
				VerifyIndex(index);

				return GetSizeImpl(index);
			}
			set
			{
				VerifyIndex(index);

				SetSizeImpl(index, value);

				VerifyStructure();
			}
		}

		public double Size { get; private set; }

		public void AddSize(double size)
		{
			AddSizeImpl(size);

			VerifyStructure();
		}

		public void AddSizeRange(long count, double size)
		{
			AddSizeRangeImpl(count, size);

			VerifyStructure();
		}

		public void Clear()
		{
			ClearImpl();

			VerifyStructure();
		}

		public long FindIndex(double offset, out double localOffset)
		{
			var cursor = NavigateToOffset(offset);

			if (cursor.IsEmpty)
			{
				localOffset = 0;

				return -1;
			}

			localOffset = offset - cursor.Offset;

			return cursor.Index;
		}

		public double FindOffset(long index)
		{
			return FindOffsetImpl(index);
		}

		public void InsertSize(long index, double size)
		{
			VerifyIndex(index, true);

			InsertSizeImpl(index, size);

			VerifyStructure();
		}

		public void InsertSizeRange(long index, int count, double size)
		{
			VerifyIndex(index, true);

			InsertSizeRangeImpl(index, count, size);

			VerifyStructure();
		}

		public void RemoveSizeAt(long index)
		{
			VerifyIndex(index);

			RemoveSizeAtImpl(index);

			VerifyStructure();
		}

		public void RemoveSizeRange(long index, long count)
		{
			VerifyRange(index, count);

			RemoveSizeRangeImpl(index, count);

			VerifyStructure();
		}

		public void ResetToAverage()
		{
			var average = AverageSize;
			var count = Count;

			Clear();

			if (count > 0)
				AddSizeRange(count, average);

			VerifyStructure();
		}
	}
}