// <copyright file="VirtualStackPanelLayoutBase.VirtualUIElementCollectionInserter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Zaaml.UI.Panels.VirtualStackPanelLayout
{
	internal partial class VirtualStackPanelLayoutBase
	{
		protected sealed class VirtualUIElementCollectionInserter
		{
			private int _headIndex;
			private int _sequentialCount;
			private State _state;

			public UIElementCollection Collection { get; private set; }

			public int Count => _state == State.SequentialAdd ? _sequentialCount : Collection.Count;

			public UIElement this[int index] => Collection[_headIndex + index];

			public void Add(UIElement item)
			{
				if (_state == State.RandomAdd)
				{
					Collection.Add(item);

					return;
				}

				if (_state == State.Initialized)
				{
					_state = State.SequentialAdd;

					var index = Collection.IndexOf(item);

					if (index == -1)
					{
						Collection.Insert(0, item);
					}
					else
					{
						if (index > 0)
							Collection.RemoveRange(0, index);
					}

					_headIndex = 0;
					_sequentialCount = 1;
				}
				else if (_state == State.SequentialAdd)
				{
					if (ReferenceEquals(Collection[_headIndex + _sequentialCount], item))
					{
						_sequentialCount++;
					}
					else
					{
						var findNextIndex = Collection.IndexOf(item);

						if (findNextIndex != -1)
						{
							Collection.RemoveRange(_headIndex + _sequentialCount, findNextIndex - (_headIndex + _sequentialCount));

							_sequentialCount++;
						}
						else
						{
							Collection.Insert(_headIndex + _sequentialCount, item);

							_sequentialCount++;
						}
					}
				}

				if (_headIndex + _sequentialCount == Collection.Count)
					CommitSequentialAdd();
			}

			private void Commit()
			{
				if (_state == State.Initialized)
				{
					Collection.Clear();

					_state = State.RandomAdd;
				}
				else if (_state == State.SequentialAdd)
				{
					CommitSequentialAdd();
				}
			}

			private void CommitSequentialAdd()
			{
				if (_state == State.SequentialAdd)
				{
					if (_headIndex > 0)
					{
						Collection.RemoveRange(0, _headIndex);

						_headIndex = 0;
					}

					if (_sequentialCount < Collection.Count)
						Collection.RemoveRange(_sequentialCount, Collection.Count - _sequentialCount);

					_state = State.RandomAdd;
				}
			}

			public void Enter(UIElementCollection collection)
			{
				_headIndex = 0;
				_sequentialCount = 0;
				_state = State.Initialized;

				Collection = collection;

				if (Collection.Count == 0)
					_state = State.RandomAdd;
			}

			public void Insert(int index, UIElement item)
			{
				Commit();

				Collection.Insert(index, item);
			}

			public void Leave()
			{
				Commit();

				Collection = null;
			}

			public void Remove(UIElement element)
			{
				var index = Collection.IndexOf(element);

				if (index == -1)
					return;

				if (_state == State.SequentialAdd)
				{
					if (index >= _headIndex && index < _headIndex + _sequentialCount)
					{
						if (index == _headIndex)
							_headIndex++;

						_sequentialCount--;
					}
				}

				Collection.RemoveAt(index);
			}

			private enum State
			{
				Initialized,
				SequentialAdd,
				RandomAdd
			}
		}
	}
}